using UnityEngine;
using UnityEngine.UI;

// 업그레이드 

// UI 버튼 누르면 동작
// 오브젝트 데이터와 상호작용(소모) => 플레이어 데이터에 데이터 처리
// 확률과 관련된거라 방법 생각해보기

// 선택지 기능 만들어둔거 아까운데 꽝 몇번시 선택지창? => 선택지도 보스 -> 필드 이동 돈 디스트로이 
// 선택지 켜질때 다른 UI 꺼지게 바꿔야함 

// 강화에 따른 외형 변화 플레이어 + UI
// UI의 경우 강화 성공시 모션 

// 펫은 자동 강화 


// 이펙트 물어보기


public class PlayerUpgrade : MonoBehaviour
{
    // 버튼 UI로 빼기 => 리펙토링 

    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _uiPlayer;

    [SerializeField] private Button UpgradeUIButton;

    [Header("강화 확률 조작")]
    [SerializeField] private float _successChanceMax = 1f;
    [SerializeField] private float _successChanceMin = 0.1f;
    [SerializeField] private float _DecreaseChance = 0.15f;


    [Header("강화 관련")]
    [SerializeField] private int _consumeCoupon = 2;            // 강화시 소모되는 플레이어레벨계수

    [SerializeField] private float _increaseFail = 1f;          // 실패시 고정 상승값
    [SerializeField] private float _increaseSuccessSet = 10f;   // 성공시 고정 상승값
    [Range(0f, 1f)]
    [SerializeField] private float _increaseSuccessPer = 0.1f;  // 성공시 퍼센트 상승값 

    // 관리용 
    [SerializeField] private float _addCriticalChance = 5f;
    [SerializeField] private float _addMoveSpeed = 0.5f;
    [SerializeField] private float _addRotateSpeed = 5f;

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



    private void Start()
    {
        UpgradeUIButton.onClick.AddListener(() => OnButtonClick("UpgradeUIButton"));

        _gameData = ManagerDontDestroy.Instance.GameData;
        if (_gameData == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start), nameof(_gameData));
        }
        _playerData = _gameData._PlayerData;
        if (_playerData == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start), nameof(_playerData));
            return;
        }
        _objectData = _gameData._ObjectData;
        if (_objectData == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start), nameof(_objectData));
            return;
        }

        if (_player == null || _uiPlayer == null)
        {
            Log.LogNull(nameof(PlayerUpgrade), nameof(Start));
        }
    }


    private void OnButtonClick(string buttonType)
    {
        if (buttonType == "UpgradeUIButton")
        {
            PlayerUpgradeClick();
        }
    }

    
    private void PlayerUpgradeClick()
    {
        if (_playerData == null)
        {
            Log.LogNull(nameof(PlayerUpgrade), nameof(PlayerUpgradeClick), nameof(_playerData));
            return;
        }
        int cost = (_playerData.PlayerLevel * _consumeCoupon) + 1;

        if (_objectData.PlayerUpgrade <= cost)
        {
            Debug.Log("강화권이 부족합니다.");
            return;
        }

        _objectData.AddPlayerUpgrade(-cost);

        // 강화 확률 계산
        float successChance = CalculateSuccessChance(_playerData.PlayerLevel);

        bool isSuccess = Random.value <= successChance;

        if (isSuccess)
        {
            PlayerUpgradeSuccess();
        }
        else
        {
            PlayerUpgradeFail();
        }
    }

    // 강화 확률 계산 
    private float CalculateSuccessChance(int level)
    {
        float decrease = Mathf.Log(level) * _DecreaseChance;
        return Mathf.Clamp(_successChanceMax - decrease, _successChanceMin, _successChanceMax);
    }

    // 성공시 고정값과 퍼센트값 비교 큰쪽으로 능력치 증가
    private void PlayerUpgradeSuccess()
    {
        _increaseSuccessSet = _increaseSuccessSet * _playerData.PlayerLevel;
        float increasePercent;
        float increaseAmount;

        increasePercent = _playerData.AttackPower * _increaseSuccessPer;
        increaseAmount = Mathf.Max(increasePercent, _increaseSuccessSet);
        _playerData.AddAttackPower(increaseAmount);

        increasePercent = _playerData.CriticalDamageMultiplier * _increaseSuccessPer;
        increaseAmount = Mathf.Max(increasePercent, _increaseSuccessSet);
        _playerData.AddCriticalDamageMultiplier(increaseAmount);

        increasePercent = _playerData.AttackSpeed * _increaseSuccessPer;
        _playerData.AddAttackSpeed(increaseAmount);

        _playerData.AddCriticalChance(_addCriticalChance);
        _playerData.AddMoveSpeed(_addMoveSpeed);
        _playerData.AddRotateSpeed(_addRotateSpeed);

        _objectData.AddPlayerUpgrade(1);

        // 에니메이션 
        // 애도 고민좀 해야겠네 이펙트만 줄지 에니메이션도 줄지 


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
            Debug.Log("강화 실패 보상: 공격력 +1");
        }
        else if (randomWeight < _attackPowerWeight + _criticalDamageWeight)
        {
            _playerData.AddCriticalDamageMultiplier(0.05f * _playerData.PlayerLevel);
            Debug.Log("강화 실패 보상: 크리티컬 배율 +0.05");
        }
        else if (randomWeight < _attackPowerWeight + _criticalDamageWeight + _moveSpeedWeight)
        {
            _playerData.AddMoveSpeed(0.1f * _playerData.PlayerLevel);
            Debug.Log("강화 실패 보상: 이동속도 +0.1");
        }
        else
        {
            _playerData.AddRotateSpeed(1f * _playerData.PlayerLevel);
            Debug.Log("강화 실패 보상: 회전속도 +1");
        }

        // 실패 강화 횟수 저장
        _playerData.AddFailUpgrade(1);
    }
 



}







