using System.Collections.Generic;
using UnityEngine;

public class CreateEnemy : MonoBehaviour
{
    // 적 15마리의 생성과 랜덤 스폰 관리

    [Header("스폰할 적 오브젝트")]
    [SerializeField] GameObject _GoEnermy = null;
    [SerializeField] private int _enermyCreate = 15;
    [SerializeField] private int _enermyCount = 10;

    [Header("적 스폰 범위")]
    [SerializeField]
    private Vector3 _enemyCenter = new Vector3(0f, 5f, 0f);

    [SerializeField] private float _enemySpawnMin = -10f;
    [SerializeField] private float _enemySpawnMax = 10f;

    private readonly Queue<GameObject> _enemyQueue = new Queue<GameObject>();
    private readonly List<GameObject> _enemyList = new List<GameObject>();

    private Transform _enemy;

    private void Awake()
    {
        if (_GoEnermy == null)
        {
            Debug.LogError("적 프리팹이 연결되지 않았습니다.");

            return;
        }
    }

    private void Start()
    {
        EnemySetting();
    }

    private void Update()
    {
        EnemySpawn();
    }

    private void EnemySetting()
    {
        if (_enemy != null)
        {
            return;
        }

        // 풀 루트 생성 → Hierarchy 정리용
        GameObject root = new GameObject("Enermy");
        _enemy = root.transform;

        // 풀에는 15개 생성
        for (int i = 0; i < _enermyCreate; i++)
        {
            GameObject enemy = Instantiate(_GoEnermy, _enemy);
            enemy.SetActive(false);

            _enemyQueue.Enqueue(enemy);
        }
    }

    private void EnemySpawn()
    {
        // 활성화된 적이 10마리면 추가 스폰하지 않음
        if (_enemyList.Count >= _enermyCount)
        {
            return;
        }

        if (_enemyQueue.Count == 0)
        {
            return;
        }

        GameObject enemy = _enemyQueue.Dequeue();

        float randX = Random.Range(_enemySpawnMin, _enemySpawnMax);
        float randZ = Random.Range(_enemySpawnMin, _enemySpawnMax);

        enemy.transform.position = new Vector3(randX, 4f, randZ) + _enemyCenter;
        enemy.transform.rotation = Quaternion.identity;

        enemy.SetActive(true);
        _enemyList.Add(enemy);
    }


    // 적이 죽었을 때 호출
    public void EnemyToPool(GameObject enemy)
    {
        if (enemy == null)
        {
            return;
        }

        // 중복 제거 체크 
        if (!_enemyList.Remove(enemy))
        {
            return;
        }

        enemy.SetActive(false);
        enemy.transform.SetParent(_enemy);
        _enemyQueue.Enqueue(enemy);
    }

    private void OnDrawGizmos()
    {
        float size = _enemySpawnMax - _enemySpawnMin;
        float offset = (_enemySpawnMin + _enemySpawnMax) * 0.5f;

        Vector3 center = new Vector3(
            _enemyCenter.x + offset,
            0f,
            _enemyCenter.z + offset
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, new Vector3(size, 0f, size));
    }
}