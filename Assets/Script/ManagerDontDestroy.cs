using UnityEngine;

public class ManagerDontDestroy : MonoBehaviour
{

    [SerializeField] private GameData _gameData;

    public static ManagerDontDestroy Instance
    {
        get;
        private set;
    }

    public GameData GameData => _gameData;


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
    }



    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
