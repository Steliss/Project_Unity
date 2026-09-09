using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    [SerializeField] private Button FieldButton;

    private CSceneManager _cSceneManager;
    private GameData _gameData;
    private PlayerData _playerData;
    private TextMeshProUGUI _timer;
    private GameObject _lossImage;
    private GameObject _WinImage;
    private GameObject _EndReport;


    private TextMeshProUGUI _playerLevel;
    private TextMeshProUGUI _attackPower;
    private TextMeshProUGUI _attackRange;
    private TextMeshProUGUI _attackSpeed;
    private TextMeshProUGUI _criticalChance;
    private TextMeshProUGUI _criticalDamageMultiplier;
    private TextMeshProUGUI _moveSpeed;
    private TextMeshProUGUI _rotateSpeed;
    private TextMeshProUGUI _petLevel;
    private TextMeshProUGUI _petAttackPower;
    private TextMeshProUGUI _upgradeCoupon;
    private TextMeshProUGUI _totalDamage;

    private TextMeshProUGUI _title;
    private TextMeshProUGUI _relicName;
    private TextMeshProUGUI _relicEffect;
    void Start()
    {
        FieldButton.onClick.AddListener(() => OnButtonClick("Field"));
        _EndReport = transform.Find("EndReport").gameObject;
        _lossImage = transform.Find("EndReport/LossImage").gameObject;
        _WinImage = transform.Find("EndReport/WinImage").gameObject;

        _gameData = ManagerDontDestroy.Instance.GameData;
        _cSceneManager = ManagerDontDestroy.Instance.SceneManager;

        if (_cSceneManager == null || _gameData == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start));
        }
        _playerData = _gameData._PlayerData;

        TextMeshSetting();
    }

    private void TextMeshSetting()
    {
        try
        {
            _timer = transform.Find("Timer/TimerText").GetComponent<TextMeshProUGUI>();
            _playerLevel = transform.Find("EndReport/PlayerLevelText").GetComponent<TextMeshProUGUI>();
            _attackPower = transform.Find("EndReport/AttackPowerText").GetComponent<TextMeshProUGUI>();
            _attackRange = transform.Find("EndReport/AttackRangeText").GetComponent<TextMeshProUGUI>();
            _attackSpeed = transform.Find("EndReport/AttackSpeedText").GetComponent<TextMeshProUGUI>();
            _criticalChance = transform.Find("EndReport/CriticalChanceText").GetComponent<TextMeshProUGUI>();
            _criticalDamageMultiplier = transform.Find("EndReport/CriticalDamageMultiplierText").GetComponent<TextMeshProUGUI>();
            _moveSpeed = transform.Find("EndReport/MoveSpeedText").GetComponent<TextMeshProUGUI>();
            _rotateSpeed = transform.Find("EndReport/RotateSpeedText").GetComponent<TextMeshProUGUI>();
            _petLevel = transform.Find("EndReport/PetLevelText").GetComponent<TextMeshProUGUI>();
            _petAttackPower = transform.Find("EndReport/PetAttackPowerText").GetComponent<TextMeshProUGUI>();
            _upgradeCoupon = transform.Find("EndReport/UseTotalCouponText").GetComponent<TextMeshProUGUI>();
            _totalDamage = transform.Find("EndReport/TotalDamageText").GetComponent<TextMeshProUGUI>();

            _title = transform.Find("EndReport/TitleText").GetComponent<TextMeshProUGUI>();
            _relicName = transform.Find("EndReport/WinImage/RelicNameText").GetComponent<TextMeshProUGUI>();
            _relicEffect = transform.Find("EndReport/WinImage/RelicEffectText").GetComponent<TextMeshProUGUI>();
        }

        catch (NullReferenceException)
        {
            Log.LogNull(nameof(BossUI), nameof(TextMeshSetting));
        }
    }

    private void EndTextUpdate()
    {
        _playerLevel.text = $"플레이어 레벨 : {_playerData.PlayerLevel}";
        _attackPower.text = $"공격력 : {_playerData.AttackPower}";
        _attackRange.text = $"사거리 : {_playerData.AttackRange}";
        _attackSpeed.text = $"공격 속도 : {_playerData.AttackSpeed}";
        _criticalChance.text = $"치명타 확률 : {_playerData.CriticalChance}";
        _criticalDamageMultiplier.text = $"치명타 배율 : {_playerData.CriticalDamageMultiplier}";
        _moveSpeed.text = $"이동속도 : {_playerData.MoveSpeed}";
        _rotateSpeed.text = $"회전속도 : {_playerData.RotateSpeed}";
        _petLevel.text = $"펫 레벨 : {_playerData.PetLevel}";
        _petAttackPower.text = $"펫 공격력 : {_playerData.PetAttackPower}";
        _upgradeCoupon.text = $"총 쿠폰사용량 : {_playerData.TotalUseCoupon}";
        _totalDamage.text = $"가한 데미지 : {_playerData.TotalDamage}";

    }

    private void RelicTextUpdate(RelicOwnedData relic)
    {
        _relicName.text = $"{relic.Name}";
        _relicEffect.text = $"유물 정보 : {relic.Description}";
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

    public void BattleEndUI(bool set, bool win, RelicOwnedData relic = null)
    {
        EndTextUpdate();
        Debug.Log($"test 가한 데미지 : {_playerData.TotalDamage}");
        _EndReport.SetActive(set);
        if(win)
        {
            _title.text = "승리!";
            RelicTextUpdate(relic);
            _lossImage.SetActive(false);
            _WinImage.SetActive(true);

        }
        else
        {
            _title.text = "패배!";
            _lossImage.SetActive(true);
            _WinImage.SetActive(false);
        }
    }


}