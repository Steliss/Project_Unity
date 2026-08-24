
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 생성된 적을 찾고 이를 타격 제거 => 보상까지 이어지게. 
// 플레이어 자료 생성의 창구  
// 



public class PlayerBattle : MonoBehaviour
{
    [SerializeField] private GameObject _player = null;

    // 플레이어 데이터 생성&전달 
    private PlayerData _playerData;
    private PlayerReward _playerReward;

    // 레이캐스트로 찾은 애너미, 레이어를 활용 enemy만 찾게 설정
    [SerializeField] private LayerMask _enemyLayerMask;

    private Enemy _enemy;
    private PlayerAnimation _playerAnimation;

    // 강화 상태 
    private bool _flagPower = false;
    private bool _flagShoot = false;


    private float _distance = 0f;


    public PlayerData PlayerData => _playerData;
    public Enemy Enemy => _enemy;

    private void Awake()
    {
        _playerData = new PlayerData();

        if (_playerData == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Awake), nameof(_playerData));
            return;
        }

    }

    private void Start()
    {
        _playerReward = GetComponent<PlayerReward>();
        if (_playerReward == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start), nameof(_playerReward));
            return;
        }

        _playerAnimation = GetComponent<PlayerAnimation>();
        if (_playerAnimation == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start), nameof(_playerAnimation));
            return;
        }

        _playerReward.Initialize(_playerData);
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
        // 전투
        //  ㄴ 보상
        // 적 탐색 

    }

    // 레이캐스트 범위 안 적 발견 및 리스트에 순서대로 배치 
    // 적 생성하고 같이 돌아서 첫 파인딩이 불안한데 사소하니 넘김
    private void EnemyFind()
    {
        // 테스트용 끝나고 지우기
        if(_enemy != null)
        {
            EnermyCheck(_enemy, Color.white);
        }

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
            Log.LogNull(nameof(PlayerBattle), nameof(PlayerMoving), nameof(_enemy));
            return;
        }

        Vector3 playerPos = _player.transform.position;
        Vector3 enemyPos = _enemy.transform.position;


        // 방향 계산
        Vector3 directionToEnemy = (enemyPos - playerPos).normalized;
        directionToEnemy.y = 0f;

        _distance = Vector3.Distance(enemyPos, playerPos);
        _playerAnimation.PlayerMoving(_distance - _playerData.AttackRange, _playerData.MoveSpeed);

        // 사거리 안이라면 이동하지 않음
        if (_distance <= _playerData.AttackRange)
        {
            return;
        }


        // 유효한 방향이 없으면 처리하지 않음
        if (directionToEnemy.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        // 적에게서 플레이어 방향으로 사거리반 만큼 이동 
        Vector3 stopPosition = enemyPos - directionToEnemy * (_playerData.AttackRange * 0.5f);
        stopPosition.y = 0f;


        Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy, Vector3.up);

        float remainingAngle = Quaternion.Angle(_player.transform.rotation, targetRotation);

        if (remainingAngle > 5f)
        {
            // 아직 적을 바라보지 않으므로 회전
            _player.transform.rotation = Quaternion.RotateTowards(_player.transform.rotation, targetRotation, _playerData.RotateSpeed * Time.deltaTime);
        }
        else
        {
            // 적을 거의 바라봤으므로 이동
            _player.transform.position = Vector3.MoveTowards(playerPos, stopPosition, _playerData.MoveSpeed * Time.deltaTime);
        }
    }


    

    private void Battle()
    {
        // 전투 모션 
        // 데미지 계산 
        // 
    }

    private void ItemDrop()
    {
        // 전투 종료 확률 계산 드랍 
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
