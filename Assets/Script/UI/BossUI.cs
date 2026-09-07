using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    [SerializeField] private Button FieldButton;

    private CSceneManager _cSceneManager;
    private GameData _gameData;
    private TextMeshProUGUI _timer;
    private GameObject _lossUI;


    void Start()
    {
        FieldButton.onClick.AddListener(() => OnButtonClick("Field"));
        _lossUI = transform.Find("LossReport").gameObject;

        _gameData = ManagerDontDestroy.Instance.GameData;
        _cSceneManager = ManagerDontDestroy.Instance.SceneManager;

        if (_cSceneManager == null || _gameData == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start));
        }

        _timer = transform.Find("Timer/TimerText").GetComponent<TextMeshProUGUI>();
    }

    private void OnButtonClick(string buttonType)
    {
        if (buttonType == "Field")
        {
            _cSceneManager.LoadScene(ESceneId.Field);
        }
    }

    private void Update()
    {
        TimerTextUpdate();
    }

    private void TimerTextUpdate()
    {
        float time = 0f;
        if (_gameData.CurrentPhase == GameData.GamePhase.Farming)
        {
            time = _gameData.FarmingTime;
        }
        else if (_gameData.CurrentPhase == GameData.GamePhase.BossBattle)
        {
            time = _gameData.BossTime;
        }

        time = Mathf.Max(0f, time - _gameData.Timer);

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        _timer.text = $"{minutes:00}:{seconds:00}";
    }

    public void BattleLossUI(bool set)
    {
        _lossUI.SetActive(set);
    }


}