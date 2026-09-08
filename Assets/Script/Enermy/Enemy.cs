using UnityEngine;
using UnityEngine.Playables;

public interface IDamageable
{
    float CurrentHP { get; }
    bool FlagIsDead { get; }
    Transform TargetTransform { get; }
    void TakeDamage(float damage);
}

public class Enemy : MonoBehaviour , IDamageable
{
    [SerializeField] private Animator _animator;

    [SerializeField] private CreateEnemy _createEnemy = null;
    [SerializeField] private CreateItem _createItem = null;
    [SerializeField] private EnemyMatarialChange _enemyMaterialChange = null;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] private float _enemyBaseHP = 10f;
    [SerializeField] private float _enemyCoefficientHP = 50f;

    [Header("중력")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckDistance = 10f;
    [SerializeField] private float _gravity = 3f;
    [SerializeField] private float _groundOffset = 0.01f;

    [Header("흔들림")]
    [SerializeField] private float _hitShake = 0.1f;
    [SerializeField] private float _maxShake = 0.3f;
    [SerializeField] private float _speedDecrease = 1f;
    [SerializeField] private float _shakeFrequency = 40f;


    private Transform _shakeTarget;
    private Vector3 _localPos;
    private float _shakeAmount;

    private GameData _gameData;

    private float _maxHP;
    private float _currentHP;
    private int HitMaterialIndex;

    private bool _flagIsDead = false;
    private bool _flagIsGrounded = false;

    private float _vertical;

    public float CurrentHP => _currentHP;
    public Transform TargetTransform => transform;
    public bool FlagIsDead => _flagIsDead;

    private void Awake()
    {
        // 게임 데이터 받아오기 
        _gameData = ManagerDontDestroy.Instance.GameData;
        if (_gameData == null)
        {
            Log.LogNull(nameof(Enemy), nameof(Start), nameof(_gameData));
        }

        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        if (_animator == null)
        {
            Log.LogNull(nameof(Enemy), nameof(Awake));
        }
        if (_skinnedMeshRenderer == null)
        {
            _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
        if (_skinnedMeshRenderer == null)
        {
            Log.LogNull(nameof(Enemy), nameof(Awake));
        }

        _shakeTarget = transform.Find("Pos");
        if (_shakeTarget == null)
        {
            Log.LogNull(nameof(Enemy), nameof(Awake));
        }

        HitMaterialIndex = _enemyMaterialChange.Material.Count - 1;
        _localPos = _shakeTarget.localPosition;
    }

    private void OnEnable()
    {
        _flagIsDead = false;
        _flagIsGrounded = false;
        _shakeAmount = 0;
        if (_shakeTarget != null)
        {
            _shakeTarget.localPosition = _localPos;
        }

        // 일단 단순하게 처리 레벨당 HP 상승곡선 다시 만들기. 
        _maxHP = _enemyBaseHP + (_gameData._ObjectData.ChestLevel * _enemyCoefficientHP);
        _currentHP = _maxHP;

        int rand = Random.Range(0, HitMaterialIndex);
        _enemyMaterialChange.ApplyMaterial(rand, _skinnedMeshRenderer);
    }

    private void Start()
    {
        if (_createEnemy == null)
        {
            Log.LogNull(nameof(Enemy), nameof(Start), nameof(_createEnemy));
        }

        if (_createItem == null || _createEnemy == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start));
        }
    }

    private void Update()
    {
        ApplyGravity();
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

        _enemyMaterialChange.ApplyMaterial(HitMaterialIndex, _skinnedMeshRenderer);
        AddHitShake();

        _currentHP -= damage;
        _gameData._PlayerData.AddTotalDamage(damage);
        Debug.Log($"가한 데미지 : {damage}");
        Debug.Log($"남은 HP : {_currentHP}");

        if (_currentHP <= 0f)
        {
            DieAnimation();

            _currentHP = 0f;
            _flagIsDead = true;

            _createItem.ItemDrop(transform.position);
        }
    }

    // 에니메이션 호출
    private void DieAnimation()
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetTrigger("tEggCrack");
    }

    // Animation Event에서 호출
    public void EggDieEvent()
    {
        _createEnemy.EnemyToPool(gameObject);
    }

    // 중력 구현 낙하 // 아이템도 써야하니 공통 스크립트 뺼지 생각
    private void ApplyGravity()
    {
        if (_flagIsGrounded)
        {
            return;
        }

        _vertical -= _gravity * Time.deltaTime;

        float fallDistance = Mathf.Abs(_vertical) * Time.deltaTime;

        bool foundGround = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _groundCheckDistance, _groundLayer);

        // 이번 프레임의 이동 거리 안에 지면이 있으면
        if (foundGround && hit.distance <= fallDistance + 0.01f || transform.position.y < hit.point.y)
        {
            Vector3 groundedPosition = transform.position;

            groundedPosition.y = hit.point.y + _groundOffset;

            transform.position = groundedPosition;

            _vertical = 0f;
            _flagIsGrounded = true;

            return;
        }

        transform.position += Vector3.down * _vertical * Time.deltaTime;
    }

    // 흔들림 효과 앤 보스도 써먹음 좋은데.
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
}

