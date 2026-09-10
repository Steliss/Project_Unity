using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FiledUI : MonoBehaviour
{

    [SerializeField] private Button useItemButton;
    [SerializeField] private Button bossSceneButton;
    [SerializeField] private Button upgradeUIOpenButton;
    [SerializeField] private Button UpgradeUIButton;
    [SerializeField] private Toggle informationChangeToggle;
    [SerializeField] private PlayerUpgrade _playerUpgrade;
    [SerializeField] private UseItem _useItem;

    [SerializeField] private float _bossSceneMoveTime = 20f;

    private CSceneManager _cSceneManager;
    private GameData _gameData;
    private ObjectData _objectData;
    private PlayerData _playerData;

    private GameObject _upgradeUI;
    private GameObject _uiPlayerDisplay;
    private GameObject _informationDisplay;
    private GameObject _itemInventory;
    private GameObject _powerTimer;

    private TextMeshProUGUI _upgradeCoupon;
    private TextMeshProUGUI _chestLevel;
    private TextMeshProUGUI _timer;
    private TextMeshProUGUI _powerTimeText;

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
    private TextMeshProUGUI _DPSCheck;
    private TextMeshProUGUI _playerUpgradeButton;

    private int _previousUpgradeCoupon = -1;
    private int _previousChestLevel = -1;

    private bool _flagInformationChangeToggle;

    void Start()
    {
        useItemButton.onClick.AddListener(() => OnButtonClick("UseItem"));
        bossSceneButton.onClick.AddListener(() => OnButtonClick("BossScene"));
        upgradeUIOpenButton.onClick.AddListener(() => OnButtonClick("UpgradeUIOpen"));
        UpgradeUIButton.onClick.AddListener(() => OnButtonClick("UpgradeUIButton"));
        informationChangeToggle.onValueChanged.AddListener(OnToggleChanged);

        _cSceneManager = ManagerDontDestroy.Instance.SceneManager;
        if (_cSceneManager == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start), nameof(_cSceneManager));
        }

        _gameData = ManagerDontDestroy.Instance.GameData;
        if (_gameData == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start));
        }

        _objectData = _gameData._ObjectData;
        _playerData = _gameData._PlayerData;

        _upgradeUI = transform.Find("BottomBar/UpgradeBackGround").gameObject;
        _uiPlayerDisplay = transform.Find("BottomBar/UpgradeBackGround/UpgradeText").gameObject;
        _informationDisplay = transform.Find("BottomBar/UpgradeBackGround/Information").gameObject;
        _itemInventory = transform.Find("BottomBar/ItemInventory").gameObject;
        _powerTimer = transform.Find("TopBar/PowerTime").gameObject;

        TextMeshSetting();
    }

    private void Update()
    {
        TopBarTextUpdate();
        BossSceneMove();
    }

    private void TopBarTextUpdate()
    {
        CouponTextUpdate();
        LevelTextUpdate();
        TimerTextUpdate();
        PowerTimer();
    }

    private void BottomBarTextUpdate()
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

        // 공격속도부분 바뀌는일 있음 10f 대신 플레이어/펫에서 변수 만들어서 날리기
        float dps = _playerData.AttackPower * (_playerData.AttackSpeed / 10f) * (1f + _playerData.CriticalChance * (_playerData.CriticalDamageMultiplier - 1f)) + _playerData.PetAttackPower / 10f;

        _DPSCheck.text = $"DPS : {dps:F1}";
    }

    private void PowerTimer()
    {
        if(_useItem.FlagPotionTimer)
        {
            _powerTimer.SetActive(true);
            _powerTimeText.text = $"남은 시간 : {_useItem.PowerTimer:F2}";
        }
        else
        {
            _powerTimer.SetActive(false);
        }
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

    private void CouponTextUpdate()
    {
        if (_upgradeCoupon == null || _objectData == null)
        {
            return;
        }

        int currentUpgradeCoupon = _objectData.PlayerUpgrade;

        if (_previousUpgradeCoupon == currentUpgradeCoupon)
        {
            return;
        }

        _previousUpgradeCoupon = currentUpgradeCoupon;
        _upgradeCoupon.text = $"강화 쿠폰: {currentUpgradeCoupon}";
    }
    private void LevelTextUpdate()
    {
        if (_chestLevel == null || _objectData == null)
        {
            return;
        }

        int currentChestLevel = _objectData.ChestLevel;

        if (_previousChestLevel == currentChestLevel)
        {
            return;
        }

        _previousChestLevel = currentChestLevel;
        _chestLevel.text = $" 상자 레벨: {currentChestLevel}";
    }
    private void OnToggleChanged(bool toggle)
    {
        _flagInformationChangeToggle = toggle;

        if (toggle)
        {
            _uiPlayerDisplay.SetActive(true);
            _informationDisplay.SetActive(false);
            Debug.Log($"기능 활성화 : {_flagInformationChangeToggle}");
        }
        else
        {
            BottomBarTextUpdate();
            _uiPlayerDisplay.SetActive(false);
            _informationDisplay.SetActive(true);
            Debug.Log($"기능 비활성화 : {_flagInformationChangeToggle}");
        }
    }



    private void OnButtonClick(string buttonType)
    {
        if (buttonType == "UseItem")
        {
            OpenInventoryUI();
        }
        else if (buttonType == "BossScene")
        {
            _cSceneManager.LoadScene(ESceneId.BossRoom);
        }
        else if (buttonType == "UpgradeUIOpen")
        {
            OpenUpgradeUI();
        }
        else if (buttonType == "UpgradeUIButton")
        {
            _playerUpgradeButton.text = $"성공 확률 : {_playerUpgrade.SuccessChance * 100:F5}";
            _playerUpgrade.PlayerUpgradeClick();
        }
    }

    private void OpenInventoryUI()
    {
        if (_itemInventory == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(OpenUpgradeUI), nameof(_itemInventory));
            return;
        }

        _itemInventory.SetActive(!_itemInventory.activeSelf);
    }




    private void OpenUpgradeUI()
    {
        if (_upgradeUI == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(OpenUpgradeUI), nameof(_upgradeUI));
            return;
        }

        _playerUpgradeButton.text = $"성공 확률 : {_playerUpgrade.SuccessChance * 100:F5}";
        _upgradeUI.SetActive(!_upgradeUI.activeSelf);
    }

    private void TextMeshSetting()
    {
        try
        {
            _upgradeCoupon = transform.Find("TopBar/UpgradeCoupon/UpgradeCouponText").GetComponent<TextMeshProUGUI>();
            _chestLevel = transform.Find("TopBar/ChestLevel/ChestLevelText").GetComponent<TextMeshProUGUI>();
            _timer = transform.Find("TopBar/Timer/TimerText").GetComponent<TextMeshProUGUI>();
            _powerTimeText = transform.Find("TopBar/PowerTime/PowerTimeText").GetComponent<TextMeshProUGUI>();
            _playerLevel = transform.Find("BottomBar/UpgradeBackGround/Information/PlayerLevelText").GetComponent<TextMeshProUGUI>();
            _attackPower = transform.Find("BottomBar/UpgradeBackGround/Information/AttackPowerText").GetComponent<TextMeshProUGUI>();
            _attackRange = transform.Find("BottomBar/UpgradeBackGround/Information/AttackRangeText").GetComponent<TextMeshProUGUI>();
            _attackSpeed = transform.Find("BottomBar/UpgradeBackGround/Information/AttackSpeedText").GetComponent<TextMeshProUGUI>();
            _criticalChance = transform.Find("BottomBar/UpgradeBackGround/Information/CriticalChanceText").GetComponent<TextMeshProUGUI>();
            _criticalDamageMultiplier = transform.Find("BottomBar/UpgradeBackGround/Information/CriticalDamageMultiplierText").GetComponent<TextMeshProUGUI>();
            _moveSpeed = transform.Find("BottomBar/UpgradeBackGround/Information/MoveSpeedText").GetComponent<TextMeshProUGUI>();
            _rotateSpeed = transform.Find("BottomBar/UpgradeBackGround/Information/RotateSpeedText").GetComponent<TextMeshProUGUI>();
            _petLevel = transform.Find("BottomBar/UpgradeBackGround/Information/PetLevelText").GetComponent<TextMeshProUGUI>();
            _petAttackPower = transform.Find("BottomBar/UpgradeBackGround/Information/PetAttackPowerText").GetComponent<TextMeshProUGUI>();
            _DPSCheck = transform.Find("BottomBar/UpgradeBackGround/Information/DPSCheckText").GetComponent<TextMeshProUGUI>();
            _playerUpgradeButton = transform.Find("BottomBar/UpgradeBackGround/UpgradeText/UpgradeButton/UpgradeButtonText").GetComponent<TextMeshProUGUI>();
        }

       //  catch (NullReferenceException)
       // {
       //     Log.LogNull(nameof(FiledUI), nameof(TextMeshSetting));
       // }
        catch (NullReferenceException e)
        {
            Debug.LogException(e, this);
        }
    }

    private void BossSceneMove()
    {
        if(_gameData.Timer > _bossSceneMoveTime && _gameData.CurrentPhase == GameData.GamePhase.BossBattle)
        {
            Debug.Log("test 시간 제한 씬이동");
            _cSceneManager.LoadScene(ESceneId.BossRoom);
        }
    }


}