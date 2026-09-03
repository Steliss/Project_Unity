using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateBoss : MonoBehaviour
{
    [SerializeField] private GameObject[] Boss = null;
    [SerializeField] private Vector3 _spawnPoint;

    [SerializeField] private PlayerAnimation _playerAnimation;

    private GameData _gameData;
    private PlayerData _playerData;

    private RewardChoiceManager _rewardChoiceManager;
    private CSceneManager _sceneManager;

    void Start()
    {
        _gameData = ManagerDontDestroy.Instance.GameData;
        _rewardChoiceManager = ManagerDontDestroy.Instance.RewardChoiceManager;
        _sceneManager = ManagerDontDestroy.Instance.SceneManager;

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

    void Update()
    {
        SpawnBoss();
    }


    public void SpawnBoss()
    {
        if(_gameData.CurrentPhase != GameData.GamePhase.BossBattle)
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

    }

    public void CoroutineStart()
    {
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
    }

}
