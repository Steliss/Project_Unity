using UnityEngine;

public class PlayerUpgrade : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _uiPlayer;
    [SerializeField] private DamageLogUI _damageLogUI;
    [SerializeField] private RewardChoiceManager _rewardChoiceManager;

    [Header("강화 확률 조작")]
    [SerializeField] private float _successChanceMax = 1f;
    [SerializeField] private float _successChanceMin = 0.1f;
    [SerializeField] private float _DecreaseChance = 0.15f;

    [Header("강화 관련")]
    [SerializeField] private int _consumeCoupon = 2;

    [SerializeField] private float _attackPowerFixed = 2f;
    [SerializeField] private float _attackPowerPerLevel = 0.2f;
    [SerializeField, Range(0f, 1f)]
    private float _attackPowerPercent = 0.05f;

    // 관리용 
    [SerializeField] private float _addCriticalDamageMultiplier = 0.025f;
    [SerializeField] private float _addAttackSpeed = 0.1f;
    //[SerializeField] private float _addCriticalChance = 0.005f;
    [SerializeField] private float _addMoveSpeed = 0.05f;
    [SerializeField] private float _addRotateSpeed = 2f;

    // 가중치 랜덤 확률 관리
    [Min(0f)]
    [SerializeField] private float _attackPowerWeight = 80f;
    [Min(0f)]
    [SerializeField] private float _criticalDamageWeight = 10f;
    [Min(0f)]
    [SerializeField] private float _moveSpeedWeight = 10f;
    [Min(0f)]
    [SerializeField] private float _rotateSpeedWeight = 5f;

    private GameData _gameData;
    private PlayerData _playerData;
    private ObjectData _objectData;

    private UIPlayerEffect _uIPlayerEffect;

    private float successChance;
    public float SuccessChance => successChance;


    private void Awake()
    {
        if (_damageLogUI == null)
        {
            Log.LogNull(nameof(PlayerUpgrade), nameof(Awake), nameof(_damageLogUI));
        }
    }

    private void Start()
    {
        _gameData = ManagerDontDestroy.Instance.GameData;
        if (_gameData == null)
        {
            Log.LogNull(nameof(PlayerUpgrade), nameof(Start), nameof(_gameData));
        }
        _playerData = _gameData._PlayerData;
        if (_playerData == null)
        {
            Log.LogNull(nameof(PlayerUpgrade), nameof(Start), nameof(_playerData));
            return;
        }
        _objectData = _gameData._ObjectData;
        if (_objectData == null)
        {
            Log.LogNull(nameof(PlayerUpgrade), nameof(Start), nameof(_objectData));
            return;
        }
        _rewardChoiceManager = ManagerDontDestroy.Instance.RewardChoiceManager;
        if (_rewardChoiceManager == null)
        {
            Log.LogNull(nameof(PlayerUpgrade), nameof(Start), nameof(_rewardChoiceManager));
        }

        if (_player == null || _uiPlayer == null)
        {
            Log.LogNull(nameof(PlayerUpgrade), nameof(Start));
        }

        _uIPlayerEffect = _uiPlayer.GetComponent<UIPlayerEffect>();
        if( _uIPlayerEffect == null )
        {
            Log.LogNull(nameof(PlayerUpgrade), nameof(Start), nameof(_uIPlayerEffect));
            return;
        }

        // 강화확률 초기값 계산 UI용
        successChance = CalculateSuccessChance(_playerData.PlayerLevel);
        //Debug.Log($"Test : {successChance}");
    }

    private void Update()
    {
        PetUpgrade();
    }

    public void PlayerUpgradeClick()
    {
        if (_playerData == null)
        {
            Log.LogNull(nameof(PlayerUpgrade), nameof(PlayerUpgradeClick), nameof(_playerData));
            return;
        }
        _playerData.UpgradeCost = (_playerData.PlayerLevel * _consumeCoupon) + 1;

        if (_objectData.PlayerUpgrade < _playerData.UpgradeCost)
        {
            _damageLogUI.AnyLog("강화권이 부족합니다.");
            //Debug.Log("강화권이 부족합니다.");
            return;
        }

        _objectData.AddPlayerUpgrade(-_playerData.UpgradeCost);
        _playerData.AddTotalCoupon(_playerData.UpgradeCost);

        if (_playerData.PlayerLevel >= 25)
        {
            _damageLogUI.AnyLog("플레이어의 최대레벨에 도달했습니다.");
            PlayerUpgradeFail();
            return;
        }

        // 강화 확률 계산
        successChance = CalculateSuccessChance(_playerData.PlayerLevel);

        bool isSuccess = Random.value <= successChance;

        if (isSuccess)
        {
            PlayerUpgradeSuccess();
            _damageLogUI.AnyLog($"플레이어 강화 <color=blue>성공! 레벨 : {_playerData.PlayerLevel}</color>");
        }
        else
        {
            // 실패 강화 횟수 저장
            _playerData.AddFailUpgrade(1);
            _damageLogUI.AnyLog($"플레이어 강화 <color=red>실패</color> 실패 횟수 : <color=red>{_playerData.FailUpgrade}</color>");

            PlayerUpgradeFail();
        }
    }

    // 강화 확률 계산 
    private float CalculateSuccessChance(int level)
    {
        float decrease = Mathf.Log(level) * _DecreaseChance;
        _playerData.UpgradeCost = (_playerData.PlayerLevel * _consumeCoupon) + 1;
        return Mathf.Clamp(_successChanceMax - decrease, _successChanceMin, _successChanceMax);
    }

    // 성공시 고정값과 퍼센트값 비교 큰쪽으로 능력치 증가
    private void PlayerUpgradeSuccess()
    {
        _uIPlayerEffect.PlayEffect();

        // 기본값은 유지하고, 현재 레벨로 이번 강화 증가량만 계산
        float fixedIncrease = _attackPowerFixed + _attackPowerPerLevel * (_playerData.PlayerLevel - 1);
        float percentIncrease = _playerData.AttackPower * _attackPowerPercent;
        float increaseAmount = Mathf.Max(fixedIncrease, percentIncrease);

        _playerData.AddAttackPower(increaseAmount);

        _playerData.AddCriticalDamageMultiplier(_addCriticalDamageMultiplier);
        _playerData.AddAttackSpeed(_addAttackSpeed);
        _playerData.AddMoveSpeed(_addMoveSpeed);
        _playerData.AddRotateSpeed(_addRotateSpeed);

        _playerData.AddPlayerLevel(1);


        Debug.Log($"강화 성공 플레이어 레벨 : {_playerData.PlayerLevel}");
    }

    //가중치 랜덤 실패 확률 계산
    private void PlayerUpgradeFail()
    {
        float totalWeight = _attackPowerWeight + _criticalDamageWeight + _moveSpeedWeight + _rotateSpeedWeight;

        if (totalWeight <= 0f)
        {
            Debug.LogWarning("설정된 가중치가 없습니다.");
            return;
        }

        // 실패시 레벨 계수만큼 곱해주기
        float randomWeight = Random.Range(0f, totalWeight);

        if (randomWeight < _attackPowerWeight)
        {
            _playerData.AddAttackPower(1f * _playerData.PlayerLevel);
            _damageLogUI.AnyLog($"보상: <color=red>공격력 {1f * _playerData.PlayerLevel:F0}</color>");
            //Debug.Log($"강화 실패 보상: 공격력 {1f * _playerData.PlayerLevel}");
        }
        else if (randomWeight < _attackPowerWeight + _criticalDamageWeight)
        {
            _playerData.AddCriticalDamageMultiplier(0.05f * _playerData.PlayerLevel);
            _damageLogUI.AnyLog($"보상: <color=red> 크리티컬 배율 {0.05f * _playerData.PlayerLevel}</color>");
            //Debug.Log($"강화 실패 보상: 크리티컬 배율 {0.05f * _playerData.PlayerLevel}");
        }
        else if (randomWeight < _attackPowerWeight + _criticalDamageWeight + _moveSpeedWeight)
        {
            _playerData.AddMoveSpeed(0.05f * _playerData.PlayerLevel);
            _damageLogUI.AnyLog($"보상: <color=red> 이동속도 {0.05f * _playerData.PlayerLevel}</color>");
            //Debug.Log($"강화 실패 보상: 이동속도 {0.05f * _playerData.PlayerLevel}");
        }
        else
        {
            _playerData.AddRotateSpeed(1f * _playerData.PlayerLevel);
            _damageLogUI.AnyLog($"보상: <color=red>회전속도 {1f * _playerData.PlayerLevel}</color>");
            //Debug.Log($"강화 실패 보상: 회전속도 {1f * _playerData.PlayerLevel}");
        }

        if(_playerData.FailUpgrade >= 10)
        {
            _playerData.AddFailUpgrade(-10);
            _rewardChoiceManager.OpenChoices();
        }
    }
 
    private void PetUpgrade()
    {
        // 자동 펫 업그레이드 
        if(_playerData.PetLevel * 3 > _objectData.PetUpgrade)
        {
            return;
        }

        _objectData.AddPetUpgrade(-_playerData.PetLevel * 3);
        _playerData.AddTotalCoupon(_playerData.PetLevel * 3);
        _playerData.AddPetAttackPower(_playerData.PetLevel);
        _damageLogUI.AnyLog($"펫 강화 성공 : <color=blue>공격력 {_playerData.PetLevel}</color>");
        Debug.Log($"펫 강화 성공 : 공격력 {_playerData.PetLevel}");

        // 공격력에 따른 레벨 상승 
        if(_playerData.PetAttackPower > _playerData.PetLevel * 20f)
        {
            _playerData.AddPetLevel(1);
        }
    }


}







