using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 객체에 붙을 에너미 데이터
    // 체력 

    [SerializeField] private GameData _gameData = null;
    [SerializeField] private CreateEnemy _createEnemy = null;
    [SerializeField] private float _enemyBaseHP = 10f;

    private float _maxHP;
    private float _currentHP;

    // 필요한가? 
    private bool _isDead = true;

    public float CurrentHP => _currentHP;


    private void Start()
    {
        if(_createEnemy == null || _gameData == null)
        {
            Log.LogNull(nameof(Enemy), nameof(Start));
        }
    }

    private void OnEnable()
    {
        // 풀에서 다시 나올 때 초기화
        _maxHP = _enemyBaseHP + 3f;
        _currentHP = _maxHP;
        _isDead = false;
    }


    public void TakeDamage(float damage)
    {
        _currentHP -= damage;

        if (CurrentHP <= 0)
        {
            _isDead = true;
            _createEnemy.EnemyToPool(this.gameObject);
        }
    }






}
