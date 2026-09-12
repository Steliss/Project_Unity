
using UnityEngine;

public class Boss : MonoBehaviour, IDamageable
{
    [SerializeField] private CreateBoss _createBoss;
    [SerializeField] private GameObject _player;
    [SerializeField] private Animator _animator;

    // 등장시 초기체력 
    [SerializeField] private float _bossBaseHP = 10f;
    [SerializeField] private float _bossCoefficientHP = 50f;

    [Header("흔들림")]
    [SerializeField] private float _hitShake = 0.1f;
    [SerializeField] private float _maxShake = 0.3f;
    [SerializeField] private float _speedDecrease = 1f;
    [SerializeField] private float _shakeFrequency = 40f;

    // 봉인 확률 
    [SerializeField, Range(0f, 1f)]    private float _deathChance = 0.1f;
    [SerializeField, Range(0f, 1f)]    private float _deathChanceIncrease = 0.1f;

    [SerializeField] private BossUI _bossUI;
    private GameData _gameData;

    private Transform _shakeTarget;
    private Vector3 _localPos;
    private float _shakeAmount;

    private float _maxHP;
    private float _currentHP;
    private int HitMaterialIndex;

    private bool _flagIsDead = false;

    public float CurrentHP => _currentHP;
    public Transform TargetTransform => transform;
    public bool FlagIsDead => _flagIsDead;

    private void Awake()
    {
        // 게임 데이터 받아오기 
        _gameData = ManagerDontDestroy.Instance.GameData;
        if (_gameData == null)
        {
            Log.LogNull(nameof(Boss), nameof(Awake), nameof(_gameData));
        }
        if (_createBoss == null)
        {
            Log.LogNull(nameof(Boss), nameof(Awake));
        }

        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        if (_animator == null)
        {
            Log.LogNull(nameof(Boss), nameof(Awake));
        }

        if(_bossUI == null)
        {
            Log.LogNull(nameof(Boss), nameof(Awake), nameof(_bossUI));
        }

        _shakeTarget = transform;
        _localPos = _shakeTarget.localPosition;
    }

    private void OnEnable()
    {
        _flagIsDead = false;
        _shakeAmount = 0;
        if (_shakeTarget != null)
        {
            _shakeTarget.localPosition = _localPos;
        }

        // 일단 단순하게 처리 레벨당 HP 상승곡선 다시 만들기. 
        _maxHP = _bossBaseHP + (_gameData._ObjectData.ChestLevel * _bossCoefficientHP);
        _currentHP = _maxHP;

        BossClearChanceTextUpdate();
        int rand = Random.Range(0, HitMaterialIndex);
    }

    private void Update()
    {
        BossPos();
    }

    private void LateUpdate()
    {
        HitShake();
    }

    public void TakeDamage(float damage)
    {
        if (_flagIsDead || !isActiveAndEnabled)
        {
            return;
        }

        AddHitShake();

        _currentHP -= damage;
        _gameData._PlayerData.AddTotalDamage(damage);
        Debug.Log($"가한 데미지 : {damage}");
        Debug.Log($"남은 HP : {_currentHP}");

        if (_currentHP <= 0f)
        {
            if (Random.value < _deathChance)
            {
                BossDie();
            }
            else
            {
                _currentHP = _maxHP;
                _deathChance = Mathf.Clamp01(_deathChance + _deathChanceIncrease);
                BossClearChanceTextUpdate();
            }
        }
    }

    private void BossPos()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _player.transform.rotation, 5f * Time.deltaTime);
    }

    private void BossDie()
    {
        _currentHP = 0f;
        _flagIsDead = true;
        transform.gameObject.SetActive(false);

        _gameData.CurrentPhase = GameData.GamePhase.None;

        _createBoss.CoroutineStart();

        //Debug.Log($"test {_gameData._PlayerData.Round}");
    }


    private void AddHitShake()
    {
        _shakeAmount = Mathf.Min(_shakeAmount + _hitShake, _maxShake);
    }
    private void HitShake()
    {

        if (_shakeAmount <= 0f)
        {
            _shakeAmount = 0f;
            _shakeTarget.localPosition = _localPos;
            return;
        }


        float shakeTime = Time.time * _shakeFrequency;

        Vector3 shakeOffset = new Vector3(Mathf.Sin(shakeTime), 0f, Mathf.Cos(shakeTime * 1.37f)) * _shakeAmount;

        _shakeTarget.localPosition = _localPos + shakeOffset;

        _shakeAmount = Mathf.MoveTowards(_shakeAmount, 0f, _speedDecrease * Time.deltaTime);
    }

    private void BossClearChanceTextUpdate()
    {
        _bossUI.BossClearChance.text = $"보스 사냥 확률 : {_deathChance * 100}%";
    }


}
