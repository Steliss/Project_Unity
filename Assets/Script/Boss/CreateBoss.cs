using System.Collections;
using UnityEngine;

public class CreateBoss : MonoBehaviour
{
    [SerializeField] private GameObject[] Boss = null;
    [SerializeField] private Vector3 _spawnPoint;
    [SerializeField] private GameObject _player;

    [SerializeField] private PlayerAnimation _playerAnimation;
    [SerializeField] private BossCameraController _bossCameraController;

    private GameData _gameData;
    private PlayerData _playerData;
    private PlayerBattle _playerBattle;

    private RewardChoiceManager _rewardChoiceManager;
    private CSceneManager _sceneManager;

    void Start()
    {
        _gameData = ManagerDontDestroy.Instance.GameData;
        _rewardChoiceManager = ManagerDontDestroy.Instance.RewardChoiceManager;
        _sceneManager = ManagerDontDestroy.Instance.SceneManager;
        _playerBattle = _player.GetComponent<PlayerBattle>();

        if (_gameData == null)
        {
            Log.LogNull(nameof(CreateBoss), nameof(Start), nameof(_gameData));
        }
        if (_rewardChoiceManager == null)
        {
            Log.LogNull(nameof(Boss), nameof(Start), nameof(_rewardChoiceManager));
        }
        _playerData = _gameData._PlayerData;
    }

    private void Update()
    {
        SpawnBoss();
    }

    public void SpawnBoss()
    {
        if (_gameData.CurrentPhase != GameData.GamePhase.BossBattle)
        {
            return;
        }

        GameObject _currentBoss = Boss[_playerData.Round];

        // 이미 살아 있는 보스가 있으면 중복 소환 방지
        if (_currentBoss.activeSelf)
        {
            return;
        }

        _currentBoss.transform.position = _spawnPoint;
        _currentBoss.SetActive(true);

        // 코루틴 카메라 엑션 
        StartCoroutine(BossCutScene(_currentBoss.transform));

    }

    public void CoroutineStart()
    {
        if (_playerData.Round == 3)
        {
            // 게임 종료

        }
        StartCoroutine(RewardCoroutine());
        StartCoroutine(AnimationCoroutine());
    }



    private IEnumerator RewardCoroutine()
    {
        _rewardChoiceManager.OpenChoices();

        yield return new WaitUntil(() => _rewardChoiceManager.IsSelecting);

    }

    private IEnumerator AnimationCoroutine()
    {
        // offset = 비율
        Vector3 offSet = new Vector3(0.1f, 0.3f, 0.1f);
        _bossCameraController.DollyCameraChange(_player.transform, offSet);

        _playerAnimation.PlayAnimation(PlayerAnimation.Animation.tPutGun);

        yield return new WaitForSeconds(3f);

        _playerAnimation.PlayAnimation(PlayerAnimation.Animation.tGreeting);

        yield return new WaitForSeconds(2f);

        _playerAnimation.PlayAnimation(PlayerAnimation.Animation.tTakeGun);

        yield return new WaitForSeconds(4f);

        _playerAnimation.PlayAnimation(PlayerAnimation.Animation.tReload);

        yield return new WaitForSeconds(5f);
        // 애니메이션 종료 후 씬 이동

        _sceneManager.LoadScene(ESceneId.Field);
        _gameData.EndBossBattle();
    }

    private IEnumerator BossCutScene(Transform boss)
    {
        _playerBattle.FlagCanBattle = true;
        _bossCameraController.DollyCameraChange(boss, Vector3.one);
        yield return new WaitForSeconds(5f);

        _playerBattle.FlagCanBattle = false;

        _bossCameraController.PlayerCameraChange();
    }


}
