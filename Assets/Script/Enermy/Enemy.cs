using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 객체에 붙을 에너미 데이터
    // 체력 



    [SerializeField] private CreateEnemy _createEnemy = null;
    [SerializeField] private float _enemyBaseHP = 10f;

    private GameData _gameData;

    private float _maxHP;
    private float _currentHP;

    // 이거로 다시 찾을지 아님 플레이어 데이터 넘길지 고민 
    private bool _flagtarget = true;

    public float CurrentHP => _currentHP;


    private void Start()
    {
        // 게임 데이터 받아오기 
        _gameData = ManagerDontDestroy.Instance.GameData;
        if(_gameData == null)
        {
            Log.LogNull(nameof(Enemy), nameof(Start), nameof(_gameData));
        }

        if (_createEnemy == null)
        {
            Log.LogNull(nameof(Enemy), nameof(Start), nameof(_createEnemy));
        }
    }

    private void OnEnable()
    {
        // 풀에서 다시 나올 때 초기화
        _maxHP = _enemyBaseHP + 3f;
        _currentHP = _maxHP;
    }


    public void TakeDamage(float damage)
    {
        _currentHP -= damage;

        if (CurrentHP <= 0)
        {
            _createEnemy.EnemyToPool(this.gameObject);
        }
    }






}
