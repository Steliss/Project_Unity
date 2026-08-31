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

    private int _playerLevel = 1;
    private int _petLevel = 1;
    private int _failUpgrade = 0;

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
    public int PlayerLevel => _playerLevel;
    public int PetLevel => _petLevel;
    public int FailUpgrade => _failUpgrade;

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

    public void AddRotateSpeed(float value)
    {
        _rotateSpeed += value;
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

    public void SetBossBattle(bool value)
    {
        _flagBossBattle = value;
    }

    public void AddPlayerLevel(int value)
    {
        _playerLevel += value;

        if (_playerLevel < 0)
        {
            _playerLevel = 0;
        }
    }
    public void AddPetLevel(int value)
    {
        _petLevel += value;

        if (_petLevel < 0)
        {
            _petLevel = 0;
        }
    }
    public void AddFailUpgrade(int value)
    {
        _failUpgrade += value;

        if (_failUpgrade < 0)
        {
            _failUpgrade = 0;
        }
    }
}


