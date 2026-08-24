using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;


public class CreateEnermy : MonoBehaviour
{
    // 해야 할거 10마리의 적 랜덤 스폰만 관리 

    [Header("스폰할 적 오브젝트")]
    [SerializeField] GameObject _GoEnermy = null;
    [SerializeField] private int _enermyCreate = 15;
    [SerializeField] private int _enermyCount = 10;

    [SerializeField] private Vector3 _enemyCenter = new Vector3(0f, 5f, 0f);
    [SerializeField] private float _enemySpwanMin = -10.0f; 
    [SerializeField] private float _enemySpwanMax = 10.0f;

    private Queue<GameObject> _enemyQueue = new Queue<GameObject>();
    private List<GameObject> _enemyList = new List<GameObject>();
    private Transform _enemy;


    private void Awake()
    {

        if (_GoEnermy == null)
        {
            Debug.Log("적 오브젝트 비었음");
            return;
        }
    }

    void Start()
    {
        EnemySetting();
    }

    void Update()
    {
        EnemySpawn();
    }


    private void EnemySetting()
    {
        if (_enemy != null)
        {
            return;
        }

        //풀 루트 생성 ->하이어라키 정리용
        GameObject root = new GameObject("Enermy");

        _enemy = root.transform;

        for (int i = 0; i < _enermyCount; i++)
        {
            GameObject Apple = Instantiate(_GoEnermy, _enemy);
            Apple.SetActive(false);

            _enemyQueue.Enqueue(Apple);
        }
    }


    private void EnemySpawn()
    {
        if (_enemyQueue.Count == 0)
        {
            // 더 생성할 생각이 없으니 리턴
            return;
        }

        GameObject apple = _enemyQueue.Dequeue();

        float randX = Random.Range(_enemySpwanMin, _enemySpwanMax);
        float randZ = Random.Range(_enemySpwanMin, _enemySpwanMax);

        apple.transform.position = new Vector3(randX, 4f, randZ) + _enemyCenter;
        apple.transform.rotation = Quaternion.identity;

        apple.SetActive(true);
        _enemyList.Add(apple);
    }

    // 죽이고 호출
    public void EnemyToPool(GameObject apple)
    {
        if (apple == null)
        {
            return;
        }

        apple.SetActive(false);
        _enemyList.Remove(apple);

        apple.transform.SetParent(_enemy);
        _enemyQueue.Enqueue(apple);
    }

}

