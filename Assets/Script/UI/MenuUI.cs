using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{

    [SerializeField] private Button StartButton;

    private CSceneManager _cSceneManager;
    private GameData _gameData;

    void Start()
    {
        StartButton.onClick.AddListener(() => OnButtonClick("StartButton"));

        _cSceneManager = ManagerDontDestroy.Instance.SceneManager;
        _gameData = ManagerDontDestroy.Instance.GameData;
        if (_cSceneManager == null || _gameData == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start));
        }
        _gameData = ManagerDontDestroy.Instance.GameData;

    }

    private void OnButtonClick(string buttonType)
    {
        if (buttonType == "StartButton")
        {
            _cSceneManager.LoadScene(ESceneId.Field);
            _gameData.CurrentPhase = GameData.GamePhase.Farming;
        }
    }

}
