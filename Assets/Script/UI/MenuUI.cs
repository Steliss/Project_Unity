using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{

    [SerializeField] private Button startButton;
    [SerializeField] private Button relicButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button closeButton;

    [SerializeField] ShowRelicUI _showRelicUI;

    private GameData _gameData;
    private CSceneManager _cSceneManager;
    private SoundManager _soundManager;

    void Start()
    {
        _cSceneManager = ManagerDontDestroy.Instance.SceneManager;
        _soundManager = ManagerDontDestroy.Instance.SoundManager;
        _gameData = ManagerDontDestroy.Instance.GameData;
        if (_cSceneManager == null || _gameData == null || _soundManager == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start));
        }

        if(_showRelicUI == null)
        {
            _showRelicUI = GetComponent<ShowRelicUI>();
        }
        if (_showRelicUI == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start));
        }

        startButton.onClick.AddListener(() => OnButtonClick("StartButton"));
        relicButton.onClick.AddListener(() => OnButtonClick("RelicButton"));
        exitButton.onClick.AddListener(() => OnButtonClick("ExitButton"));
        closeButton.onClick.AddListener(() => OnButtonClick("CloseUIButton"));

        _soundManager = ManagerDontDestroy.Instance.SoundManager;
        _soundManager.BGMAudio.Stop();
        _soundManager.BGMSoundPlay(SoundManager.BGM.Title);
    }

    private void OnButtonClick(string buttonType)
    {
        Debug.Log($"¹öÆ° Å¬¸¯µÊ: {buttonType}");

        if (buttonType == "StartButton")
        {
            _soundManager.SFXStartMenuPlay();
            _cSceneManager.LoadScene(ESceneId.Field);
            _gameData.CurrentPhase = GameData.GamePhase.Farming;
        }
        else if(buttonType == "RelicButton")
        {
            _soundManager.SFXStartMenuPlay();
            _showRelicUI.ToggleRelicUI();
        }
        else if (buttonType == "CloseUIButton")
        {
            _soundManager.SFXGunfirePlay();
            _showRelicUI.ToggleRelicUI();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            OnButtonClick("CloseUIButton");
        }
    }
}
