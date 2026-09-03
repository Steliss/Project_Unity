using UnityEngine;

public class ManagerDontDestroy : MonoBehaviour
{

    [SerializeField] private GameData _gameData;
    [SerializeField] private CSceneManager _sceneManager;
    [SerializeField] private RewardChoiceManager _rewardChoiceManager;

    public static ManagerDontDestroy Instance
    {
        get;
        private set;
    }

    public GameData GameData => _gameData;
    public CSceneManager SceneManager => _sceneManager;
    public RewardChoiceManager RewardChoiceManager => _rewardChoiceManager;

    private void Awake()
    {
        // 중복 매니저 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 씬이 변경되어도 유지
        DontDestroyOnLoad(gameObject);

        if (_gameData == null)
        {
            _gameData = GetComponentInChildren<GameData>(true);
        }

        if (_gameData == null)
        {
            Log.LogNull(nameof(ManagerDontDestroy), nameof(Awake), nameof(_gameData));
        }

        if (_sceneManager == null)
        {
            _sceneManager = GetComponentInChildren<CSceneManager>(true);
        }

        if (_sceneManager == null)
        {
            Log.LogNull(nameof(ManagerDontDestroy), nameof(Awake), nameof(_sceneManager));
        }

        if (_rewardChoiceManager == null)
        {
            _rewardChoiceManager = GetComponentInChildren<RewardChoiceManager>(true);
        }

        if (_rewardChoiceManager == null)
        {
            Log.LogNull(nameof(ManagerDontDestroy), nameof(Awake), nameof(_rewardChoiceManager));
        }
    }



    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
