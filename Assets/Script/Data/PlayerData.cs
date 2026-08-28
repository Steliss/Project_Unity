using System;
using UnityEngine;


// 플레이어 자료 공간


[Serializable]
public class PlayerData
{

    [SerializeField] private float _attackPower = 50f;
    [SerializeField] private float _attackSpeed = 10f;
    [SerializeField] private float _attackRange = 5f;
    [SerializeField] private float _findRange = 100f;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _rotateSpeed = 90f;
    [SerializeField] private float _criticalChance = 0.1f;
    [SerializeField] private float _criticalDamageMultiplier = 1.5f;

    [SerializeField] private bool _flagBossBattle = false;
    
    [SerializeField] private float _petAttackPower = 1f;


    public float AttackPower => _attackPower;
    public float AttackSpeed => _attackSpeed;
    public float AttackRange => _attackRange;
    public float FindRange => _findRange;
    public float MoveSpeed => _moveSpeed;
    public float RotateSpeed => _rotateSpeed;
    public float CriticalChance => _criticalChance;
    public float CriticalDamageMultiplier => _criticalDamageMultiplier;
    public bool FlagBossBattle => _flagBossBattle;
    public float PetAttackPower => _petAttackPower;


    public void AddAttackPower(float value)
    {
        _attackPower += value;
    }

    public void AddAttackSpeed(float value)
    {
        _attackSpeed += value;
    }

    public void AddAttackRange(float value)
    {
        _attackRange += value;
    }

    public void AddMoveSpeed(float value)
    {
        _moveSpeed += value;
    }
    public void AddCriticalChance(float value)
    {
        _criticalChance += value;
    }
    public void AddCriticalDamageMultiplier(float value)
    {
        _criticalDamageMultiplier += value;
    }
    public void AddPetAttackPower(float value)
    {
        _petAttackPower += value;
    }

    
    // 스타트 단계에서만 받아올거고. 상태 변환이 필요하니 이게 맞네 
    public bool BossBattle(bool valve)
    {
        _flagBossBattle = !valve;
        return _flagBossBattle!;
    }

}

