
using UnityEngine;
using UnityEngine.Playables;

public class PetCondition : MonoBehaviour
{
    [SerializeField] private GameObject _player = null;

    // 공전 설정
    [SerializeField] private float _orbitRadius = 3f;
    [SerializeField] private float _orbitSpeed = 12f;
    [SerializeField] private float _rotateSpeed = 240f;

    // 비행 설정
    [SerializeField] private float _flyingHeight = 4f;
    [SerializeField] private float _floatingHeight = 2f;
    [SerializeField] private float _floatingSpeed = 2f;

    // 지상 및 착륙 설정
    //[SerializeField] private LayerMask _groundLayer;
    //[SerializeField] private float _groundCheckDistance = 10f;
    [SerializeField] private float _landingSpeed = 3f;

    // 복귀 설정
    [SerializeField] private float _teleportDistance = 20f;

    private PlayerData _playerData;
    private GameData _gameData;
    private PlayerBattle _playerBattle;
    private PetAnimation _petAnimation;

    private IDamageable _target;

    private bool _flagBattle = false;
    private bool _flagFlying = false;
    private bool _flagLanding = false;
    private bool _flagAnimation = false;

    private float _orbitAngle;

    private float battleTimer = 0f;
    private float animationTimer = 0f;

    private Vector3 tarPos;

    private enum Condition
    {
        Idle,
        Move,
        Play,
        None
    }

    private Condition _petCondition = new Condition();


    private void Awake()
    {
        if(_player == null)
        {
            Log.LogNull(nameof(PetCondition), nameof(Awake), nameof(_player));
            return;
        }

        if (_playerBattle == null)
        {
            _playerBattle = _player.GetComponent<PlayerBattle>();
            return;
        }
        if (_playerBattle == null)
        {
            Log.LogNull(nameof(PetCondition), nameof(Awake), nameof(_playerBattle));
            return;
        }


    }

    void Start()
    {
        _gameData = ManagerDontDestroy.Instance.GameData;
        _playerData = _gameData._PlayerData;

        _petAnimation = GetComponent<PetAnimation>();
        if (_petAnimation == null)
        {
            Log.LogNull(nameof(PetCondition), nameof(Start), nameof(_petAnimation));
            return;
        }

        _petCondition = Condition.Move;

        // 위치 초기값 넣어주기. 
    }

    private bool IsEnemyValid()
    {
        return _target != null && _target.CurrentHP > 0f && !_target.FlagIsDead;
    }

        
    void Update()
    {
        if(_gameData.CurrentPhase != GameData.GamePhase.None)
        {
            _target = _playerBattle._Target;
        }

        if (battleTimer > 10f)
        {
            _flagBattle = true;
        }

        DragonIdle();
        DragonPlay();
        DragonPetBattle();
        DragonPetMove();


        if (Input.GetKeyDown(KeyCode.P))
        {
            _playerData.AddPlayerLevel(1);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            _playerData.AddPlayerLevel(-1);
        }

    }


    // 아이들 모션 트리거 
    // 애니메이션 재생후 무빙으로 
    // 날고 있다면 땅으로 내리기
    private void DragonIdle()
    {
        if (_player == null || _petCondition != Condition.Idle || _flagBattle)
        {
            return;
        }

        if (_flagFlying)
        {
            _petAnimation.PetMoving(1f);
            DragonPetLanding();

            if (_flagFlying)
            {
                return;
            }
        }

        PetLookRotate(_player.transform.position);
        _petAnimation.PetMoving(0f);
        int motion = Random.Range(0, 2);


        if (!_flagAnimation)
        {
            _petAnimation.PlayAnimation(motion == 0 ? PetAnimation.Animation.IdleSmell : PetAnimation.Animation.IdleYaw);
            _flagAnimation = true;
        }
        else
        {
            animationTimer += Time.deltaTime;
        }
        
        if(animationTimer > 3.5f)
        {
            _petCondition = Condition.Move;

            _flagAnimation = false;
            animationTimer = 0;
        }

    }

    private void DragonPlay()
    {
        if (_player == null || _petCondition != Condition.Play || _flagBattle)
        {
            return;
        }

        if (_flagFlying)
        {
            _petAnimation.PetMoving(1f);
            DragonPetLanding();

            if (_flagFlying)
            {
                return;
            }
        }

        PetLookRotate(_player.transform.position);
        _petAnimation.PetMoving(0f);
        int motion = Random.Range(0, 2);


        if (!_flagAnimation)
        {
            _petAnimation.PlayAnimation(motion == 0 ? PetAnimation.Animation.PetVr : PetAnimation.Animation.Confuse);
            _flagAnimation = true;
        }
        else
        {
            animationTimer += Time.deltaTime;
        }

        if (animationTimer > 8.3f)
        {
            _petCondition = Condition.Move;

            _flagAnimation = false;
            animationTimer = 0;
        }
    }



    private void DragonPetBattle()
    {
        battleTimer += Time.deltaTime;

        if (!_flagBattle || !IsEnemyValid())
        {
            return;
        }
        //Debug.Log("펫 배틀 진입");

        float toDamage = _playerData.PetAttackPower;

        if(battleTimer > 20f)
        {
            DragonBattleConditionClear();
            Debug.Log("펫 배틀 타임 초과");
            return;
        }


        if (_flagFlying)
        {
            // 시간나면 플레이어처럼 봤을때 공격하기로 바꾸기
            PetLookRotate(_target.TargetTransform.position);

            if (!_flagAnimation)
            {
                _petAnimation.PlayAnimation(PetAnimation.Animation.FireAttack);

                _flagAnimation = true;
                animationTimer = 0f;
            }
            // 클리어 전에 딜레이 시간 주기
            animationTimer += Time.deltaTime;

            if (animationTimer > 1f)
            {
                _target.TakeDamage(toDamage);
                DragonBattleConditionClear();
            }

        }
        // 지상
        else
        {
            //땅일때 
            Vector3 normal = (_target.TargetTransform.position - transform.position).normalized;
            Vector3 tarpos = _target.TargetTransform.position - normal * 1f;
            tarpos.y = GroundHeight();
            transform.position = Vector3.MoveTowards(transform.position, tarpos, _playerData.MoveSpeed * 2 * Time.deltaTime);
            PetLookRotate(_target.TargetTransform.position);



            // 공격 
            if ((transform.position - tarpos).sqrMagnitude <= 0.01f)
            {
                int rand = -1;

                if (!_flagAnimation)
                {
                    rand = Random.Range(0, 2);
                    _petAnimation.PlayAnimation(rand == 0 ? PetAnimation.Animation.PawR : PetAnimation.Animation.PawL);
                    _flagAnimation = true;
                }

                animationTimer += Time.deltaTime;
                if (animationTimer > 1f)
                {
                    _target.TakeDamage(toDamage);
                    DragonBattleConditionClear();
                }
            }
        }


    }

    private void DragonBattleConditionClear()
    {
        _flagBattle = false;
        _flagFlying = true;
        _flagAnimation = false;
        battleTimer = 0f;
        animationTimer = 0f;
        _petAnimation.PetMoving(0f);



        int rand = Random.Range(0, 3);

        if (rand == 0)
        {
            _petCondition = Condition.Idle;
        }
        else if (rand == 1)
        {
            _petCondition = Condition.Play;
        }
        else
        {
            _petCondition = Condition.Move;
        }

        Debug.Log("Battle Clear");
        Debug.Log($"pet cpondition : {_petCondition}");
    }




    private void DragonPetMove()
    {

        if (_player == null || _petCondition != Condition.Move || _flagBattle)
        {
            return;
        }

        Vector3 playerPosition = _player.transform.position;

        // 플레이어 거리 계산 
        float distance = (transform.position - playerPosition).sqrMagnitude;
        float teleportDistance = _teleportDistance * _teleportDistance;

        // 멀어지면 플레이어 주변으로 즉시 복귀
        if (distance > teleportDistance)
        {
            transform.position = playerPosition + Vector3.up * _flyingHeight;
            return;
        }


        // 시간에 따른 공전각도 
        _orbitAngle = Mathf.Repeat(_orbitAngle + _orbitSpeed * Time.deltaTime, 360f);   

        float angleRadian = _orbitAngle * Mathf.Deg2Rad;

        // 회전 위치값 만들고 대입
        Vector3 orbitOffset = new Vector3(Mathf.Cos(angleRadian), 0f, Mathf.Sin(angleRadian)) * _orbitRadius;
        tarPos = playerPosition + orbitOffset;


        if (_flagLanding)
        {
            _petAnimation.PetMoving(1f);
            DragonPetLanding();
            return;
        }

        if (_flagFlying)
        {
            // 날고 있다면 Y축 높이 조절
            float floatingOffset = Mathf.Sin(Time.time * _floatingSpeed) * _floatingHeight;
            tarPos.y = playerPosition.y + _flyingHeight + floatingOffset;
            //
            _petAnimation.PetMoving(1f);
            //
        }
        else
        {
            tarPos.y = GroundHeight();
            //
            _petAnimation.PetMoving(0.5f);
            //
        }

        PetLookRotate(tarPos);
        transform.position = Vector3.MoveTowards(transform.position, tarPos, _playerData.MoveSpeed * Time.deltaTime);
    }



    private void DragonPetLanding()
    {
        // 이 착륙 함수 
        Vector3 landingPos = tarPos;
        float groundHeight = GroundHeight();
        landingPos.y = _flagFlying ? groundHeight : groundHeight + _flyingHeight;

        PetLookRotate(landingPos);
        transform.position = Vector3.MoveTowards(transform.position, landingPos, _landingSpeed * Time.deltaTime);

        float Height = Mathf.Abs(transform.position.y - landingPos.y);

        if (Height <= 0.001f)
        {
            transform.position = landingPos;

            _flagFlying = !_flagFlying;
            _flagLanding = false;
            //Debug.Log("이착륙 완료");
        }
    }

    private void PetLookRotate(Vector3 rot)
    {
        Vector3 dir = (rot - transform.position).normalized;

        if (dir == Vector3.zero)
        {
            return;
        }

        Quaternion tarRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, tarRot, _rotateSpeed * Time.deltaTime);
    }



    private float GroundHeight()
    {
        return _player.transform.position.y;
    }







}
