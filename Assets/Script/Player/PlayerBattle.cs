using UnityEngine;

// 생성된 적을 찾고 이를 타격 제거 => 보상까지 이어지게. 
// 플레이어 자료 생성의 창구  
// 



public class PlayerBattle : MonoBehaviour
{
    [SerializeField] private bool _flagTestLog = false;

    [SerializeField] private GameObject _player;
    [SerializeField] private CreateItem _createItem = null;

    // 레이캐스트로 찾은 애너미, 레이어를 활용 enemy만 찾게 설정
    [SerializeField] private LayerMask _enemyLayerMask;

    // 데이터 생성&전달 
    // 생성 끝나고 넣어주기 
    private GameData _gameData;
    private PlayerData _playerData;
    private ObjectData _objectData;
    private Enemy _enemy;
    private PlayerAnimation _playerAnimation;

    // 강화 상태 여기 두기 애매한거 같은데 흠.
    private bool _flagPower = false;
    private bool _flagShoot = false;
    private bool _flagCanBattle = false;

    private float _distance = 0f;
    private float _attakDuration = 0f;

    public Enemy _Enemy => _enemy;



    private void Start()
    {

        _gameData = ManagerDontDestroy.Instance.GameData;
        if (_gameData == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start), nameof(_gameData));
        }



        _playerAnimation = GetComponent<PlayerAnimation>();
        if (_playerAnimation == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start), nameof(_playerAnimation));
            return;
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

        if(_createItem == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start));
        }


    }


    private void Update()
    {
        // 적 탐색 
        if (_enemy == null)
        {
            EnemyFind();
        }

        // 테스트
        if (Input.GetMouseButtonDown(0))
        {   
            EnemyFind();
        }


        PlayerMoving();
        Battle();
        //  ㄴ 보상
        

    }

    // 레이캐스트 범위 안 적 발견 및 리스트에 순서대로 배치 
    // 적 생성하고 같이 돌아서 첫 파인딩이 불안한데 사소하니 넘김
    private void EnemyFind()
    {
        // 기존 삭제 및 탐색후 저장
        _enemy = null;
        Collider[] detectedColliders = Physics.OverlapSphere(_player.transform.position, _playerData.FindRange, _enemyLayerMask);

        float closestDistance = float.MaxValue;

        // 찾은 배열 순회
        foreach (Collider detectedCollider in detectedColliders)
        {
            Enemy enemy = detectedCollider.GetComponentInParent<Enemy>();

            if (enemy == null)
            {
                continue;
            }

            // 테스트용 끝나고 지우기
            EnermyCheck(enemy, Color.white);

            float distance = (enemy.transform.position - _player.transform.position).sqrMagnitude;

            // 거리가 순회 거리보다 클 경우
            if (distance >= closestDistance)
            {
                continue;
            }

            // 작을경우 0번 리스트 갱신 및 애너미 추가
            closestDistance = distance;
            _enemy = enemy;
        }

        if (_enemy != null)
        {
            EnermyCheck(_enemy, Color.red);
        }
    }

    private void PlayerMoving()
    {
        if (_enemy == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(PlayerMoving));
            return;
        }

        Vector3 playerPos = _player.transform.position;
        Vector3 enemyPos = _enemy.transform.position;


        // 방향 계산
        Vector3 directionToEnemy = (enemyPos - playerPos).normalized;
        directionToEnemy.y = 0f;

        _distance = Vector3.Distance(enemyPos, playerPos);
        _playerAnimation.PlayerMoving(_distance - _playerData.AttackRange, _playerData.MoveSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy, Vector3.up);
        float remainingAngle = Quaternion.Angle(_player.transform.rotation, targetRotation);

        // 유효한 방향이 없으면 처리하지 않음
        if (directionToEnemy.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        if (remainingAngle > 5f)
        {
            // 아직 적을 바라보지 않으므로 회전
            _player.transform.rotation = Quaternion.RotateTowards(_player.transform.rotation, targetRotation, _playerData.RotateSpeed * Time.deltaTime);
            _flagCanBattle = false;
            return;
        }

        _flagCanBattle = true;

        // 사거리 안이라면 이동하지 않음
        if (_distance <= _playerData.AttackRange)
        {
            return;
        }

        // 적에게서 플레이어 방향으로 사거리반 만큼 이동 
        Vector3 stopPosition = enemyPos - directionToEnemy * (_playerData.AttackRange * 0.5f);
        stopPosition.y = 0f;

        // 이동
        _player.transform.position = Vector3.MoveTowards(playerPos, stopPosition, _playerData.MoveSpeed * Time.deltaTime);

    }




    private void Battle()
    {
        // 테스트로그
        if (_flagTestLog)
        {
            Debug.Log(_attakDuration);
            Debug.Log(_flagShoot);
        }

        // 적이 비었으면 리턴
        if (_enemy == null)
        {
            _flagShoot = false;
            return;
        }

        if(!_flagCanBattle)
        {
            //Log(_flagCanBattle);
            return;
        }

        _attakDuration += Time.deltaTime;
        // 전투 모션 
        _playerAnimation.PlayerShoot(_flagShoot);

        // 사거리 밖이면 리턴
        if (_distance > _playerData.AttackRange)
        {
            _flagShoot = false;
            return;
        }

        // 파워 업의 경우 
        float duration = 10f;
        if (_flagPower)
        {
            duration = 1f;
        }

        // 공격 주기가 안됬음 리턴  
        if (_attakDuration * _playerData.AttackSpeed < duration)
        {
            return;
        }

        _flagShoot = true;

        // 데미지 계산 
        float toDamage = _playerData.AttackPower;

        // 크리 구현
        bool isCritical = Random.value < _playerData.CriticalChance;

        if (isCritical)
        {
            toDamage *= _playerData.CriticalDamageMultiplier;

            Debug.Log($"치명타");
        }

        if (_flagTestLog)
        {
            Debug.Log($"데미지를 줬습니다 {toDamage}");
        }

        _enemy.TakeDamage(toDamage);
        _attakDuration = 0f;

        if (_enemy.CurrentHP <= 0.1f)
        {
            _flagShoot = false;

            // 아이템 드랍
            _createItem.ItemDrop(_enemy.transform.position);

            _enemy = null;
        }

    }








    // 테스트용 끝나고 지우기
    private void EnermyCheck(Enemy enemy, Color color)
    {
        Renderer enemyRenderer = enemy.GetComponent<Renderer>();

        if (enemyRenderer == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(EnermyCheck), nameof(enemyRenderer));
            return;
        }

        enemyRenderer.material.color = color;
    }
}
