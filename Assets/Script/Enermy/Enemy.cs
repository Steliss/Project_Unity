using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 객체에 붙을 에너미 데이터
    // 체력 



    [SerializeField] private CreateEnemy _createEnemy = null;
    [SerializeField] private CreateItem _createItem = null;
    [SerializeField] private float _enemyBaseHP = 10f;

    private GameData _gameData;

    private float _maxHP;
    private float _currentHP;

    private bool _flagIsDead = false;

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

        if (_createItem == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start));
        }
    }

    private void OnEnable()
    {
        _flagIsDead = false;
        // 풀에서 다시 나올 때 초기화
        _maxHP = _enemyBaseHP + 3f;
        _currentHP = _maxHP;
    }


    public void TakeDamage(float damage)
    {
        if (_flagIsDead || !isActiveAndEnabled)
        {
            return;
        }

        _currentHP -= damage;
        Debug.Log($"가한 데미지 : {damage}");
        Debug.Log($"남은 HP : {_currentHP}");

        if (_currentHP <= 0f)
        {
            _currentHP = 0f;
            _flagIsDead = true;

            _createItem.ItemDrop(transform.position);
            _createEnemy.EnemyToPool(gameObject);
        }
    }






}
