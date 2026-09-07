using System.Collections;
using UnityEngine;

public class CreateBoss : MonoBehaviour
{
    [SerializeField] private GameObject[] Boss = null;
    [SerializeField] private Vector3 _spawnPoint;
    [SerializeField] private GameObject _player;

    [SerializeField] private PlayerAnimation _playerAnimation;
    [SerializeField] private BossCameraController _bossCameraController;
    [SerializeField] private BossUI _bossUI;

    private GameData _gameData;
    private ObjectData _objectData;
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
        _objectData = _gameData._ObjectData;
    }

    private void Update()
    {
        SpawnBoss();

        if(_gameData.CurrentPhase == GameData.GamePhase.GameOver)
        {
            StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        Debug.Log("제한시간 초과 / 게임 오버");

        // 플레이어 애니메이션 애도 분기 
        _playerAnimation.PlayAnimation(PlayerAnimation.Animation.tDieB);
        yield return new WaitForSeconds(3f);

        // 패배 UI => 점수 및 통계 (승리씬도 가야하네?)
        _bossUI.BattleLossUI(true);
        yield return new WaitUntil(() => Input.anyKeyDown);
        _bossUI.BattleLossUI(false);

        // 플레이어 데이터 초기화 타이머는 gamedata에서 처리 
        _playerData.ResetState();
        _objectData.ResetState();

        _sceneManager.LoadScene(ESceneId.Menu);
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
            // 승리 
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

        // 스마일 분기 추가
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
