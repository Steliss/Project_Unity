using UnityEngine;

public class GameData : MonoBehaviour
{
    public enum GamePhase
    {
        None,
        Farming,
        BossBattle,
        GameOver
    }

    [SerializeField] private float _farmingTime = 60f;   // 10분
    [SerializeField] private float _bossTime = 15f;      // 2분 30초

    private GamePhase _currentPhase;

    private ObjectData _objectData;
    private PlayerData _playerData;
    private CSceneManager _cSceneManager;
    private float _timer = 0f;

    private bool _flagBossBaltte = false;

    public ObjectData _ObjectData => _objectData;
    public PlayerData _PlayerData => _playerData;
    public CSceneManager CSceneManager => _cSceneManager;
    public float Timer => _timer;
    public GamePhase CurrentPhase
    {
        get => _currentPhase;
        set => _currentPhase = value;
    }

    public float FarmingTime => _farmingTime;
    public float BossTime => _bossTime;
    public bool FlagBossBattle => _flagBossBaltte;

    private void Awake()
    {
        _playerData = new PlayerData();
        if (_playerData == null)
        {
            Log.LogNull(nameof(GameData), nameof(Awake), nameof(_playerData));
            return;
        }

        _objectData = new ObjectData();
        if (_objectData == null)
        {
            Log.LogNull(nameof(GameData), nameof(Awake), nameof(_objectData));
            return;
        }
    }

    private void Start()
    {
        _cSceneManager = ManagerDontDestroy.Instance.SceneManager;
        if(_cSceneManager == null)
        {
            Log.LogNull(nameof(GameData), nameof(Start), nameof(_cSceneManager));
        }
    }

    private void Update()
    {
        GameTimer();
    }

    private void GameTimer()
    {
        // 파밍시간  : 10분
        // 보스잡는시간 : 2분30초

        // 시간안에 보스 못잡으면 게임 오버 <= 메뉴 이동 

        if(_currentPhase == GamePhase.None || _currentPhase == GamePhase.GameOver)
        {
            return;
        }

        _timer += Time.deltaTime;

        switch (_currentPhase)
        {
            case GamePhase.Farming:
                if (_timer >= _farmingTime)
                {
                    StartBossBattle();
                }
                break;

            case GamePhase.BossBattle:
                if (_timer >= _bossTime)
                {
                    GameOver();
                }
                break;
        }
    }

    private void StartBossBattle()
    {
        SetBossBattle();
        _currentPhase = GamePhase.BossBattle;
        _timer = 0f;

        Debug.Log("보스전 타이머");
    }

    public void EndBossBattle()
    {
        SetBossBattle();
        _playerData.AddRound(1);
        _currentPhase = GamePhase.Farming;
        _timer = 0f;
    }


    private void GameOver()
    {
        // 페이즈만 변화 처리는 CreateBoss에서 승리조건과 같이
        _currentPhase = GamePhase.GameOver;
        _timer = 0f;
    }

    public void SetBossBattle()
    {
        _flagBossBaltte = !_flagBossBaltte;
    }


}

