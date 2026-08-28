using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FiledUI : MonoBehaviour
{
    
    [SerializeField] private Button UseItemButton;
    [SerializeField] private Button BossSceneButton;
    [SerializeField] private Button UpgradeUIOpenButton;


    private CSceneManager _cSceneManager;
    private ObjectData _objectData;

    private GameObject _upgradeUI;
    private TextMeshProUGUI _upgradeCoupon;
    private TextMeshProUGUI _chestLevel;

    private int _previousUpgradeCoupon = -1;
    private int _previousChestLevel = -1;


    void Start()
    {
        UseItemButton.onClick.AddListener(() => OnButtonClick("UseItem"));
        BossSceneButton.onClick.AddListener(() => OnButtonClick("BossScene"));
        UpgradeUIOpenButton.onClick.AddListener(() => OnButtonClick("UpgradeUIOpen"));

        ManagerDontDestroy manager = ManagerDontDestroy.Instance;

        _cSceneManager = manager.SceneManager;
        if (_cSceneManager ==null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start), nameof(_cSceneManager));
        }
        _objectData = manager.GameData._ObjectData;
        if (_cSceneManager == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start), nameof(_objectData));
        }


        // 인덱스 서치 부분 다듬을 필요 있음 주의!
        _upgradeUI = transform.GetChild(3).gameObject;
        if(_upgradeUI == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start), nameof(_upgradeUI));
        }

        // 애도 다듬을 방법 생각하기
        _upgradeCoupon = transform.Find("UpgradeCouponText").GetComponent<TextMeshProUGUI>();
        if (_upgradeCoupon == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start), nameof(_upgradeCoupon));
        }
        _chestLevel = transform.Find("ChestLevelText").GetComponent<TextMeshProUGUI>();
        if (_upgradeCoupon == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start), nameof(_upgradeCoupon));
        }
    }

    private void Update()
    {
        CouponTextUpdate();
        LevelTextUpdate();
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

        int currentUpgradeCoupon = _objectData.ChestLevel;

        if (_previousChestLevel == currentUpgradeCoupon)
        {
            return;
        }

        _previousChestLevel = currentUpgradeCoupon;

        _chestLevel.text = $"강화 쿠폰: {currentUpgradeCoupon}";
    }


    private void OnButtonClick(string buttonType)
    {
        if (buttonType == "UseItem")
        {

        }
        else if (buttonType == "BossScene")
        {
            _cSceneManager.LoadScene(ESceneId.BossRoom);
        }
        else if (buttonType == "UpgradeUIOpen")
        {
            OpenUpgradeUI();
        }
    }



    private void OpenUpgradeUI()
    {
        if (_upgradeUI == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(OpenUpgradeUI), nameof(_upgradeUI));
            return;
        }

        _upgradeUI.SetActive(!_upgradeUI.activeSelf);

    }




}