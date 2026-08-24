
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
    private Enemy _enemy;
    [SerializeField] private LayerMask _enemyLayerMask;

    // 강화 상태 
    private bool _isPower = false;


    public PlayerData PlayerData => _playerData;

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

        _playerReward.Initialize(_playerData);
    }


    private void Update()
    {
        // 초기 시행 
        if (_enemy == null)
        {
            EnemyFind();
        }

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

        float distance = Vector3.Distance(enemyPos, playerPos);

        // 사거리 안이라면 이동하지 않음
        if (distance <= _playerData.AttackRange)
        {
            return;
        }

        // 방향 계산
        Vector3 directionToEnemy = (playerPos- enemyPos).normalized;

        // 적에게서 플레이어 방향으로 사거리만큼 떨어진 위치 보정값 3f
        Vector3 stopPosition = enemyPos - directionToEnemy * (_playerData.AttackRange - 3f);
        stopPosition.y = 0f;

        _player.transform.position = Vector3.MoveTowards(playerPos, stopPosition, _playerData.MoveSpeed * Time.deltaTime);

        // 속도에 따른 애니메이션 
        PlayerAnimation.PlayerMoving(_playerData.MoveSpeed);
    }

    private void Battle()
    {

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
