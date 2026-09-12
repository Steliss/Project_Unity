using System;
using UnityEngine;


// 플레이어 자료 공간


[Serializable]
public class PlayerData
{
    [SerializeField] private float _attackPower = 10f;
    [SerializeField] private float _attackSpeed = 3f;
    [SerializeField] private float _attackRange = 5f;
    [SerializeField] private float _findRange = 100f;
    [SerializeField] private float _moveSpeed = 1.5f;
    [SerializeField] private float _rotateSpeed = 90f;
    [SerializeField] private float _criticalChance = 0.1f;
    [SerializeField] private float _criticalDamageMultiplier = 1.5f;
    
    [SerializeField] private float _petAttackPower = 1f;

    private int _playerLevel = 1;
    private int _petLevel = 1;
    private int _failUpgrade = 0;
    private int _round = 0;

    private float _totalDamage = 0f;
    private int _totalUseCoupon = 0;

    private float[] _initialStats;
    private int[] _initialLevel;
    
    public float AttackPower => _attackPower;
    public float AttackSpeed => _attackSpeed;
    public float AttackRange => _attackRange;
    public float FindRange => _findRange;
    public float MoveSpeed => _moveSpeed;
    public float RotateSpeed => _rotateSpeed;
    public float CriticalChance => _criticalChance;
    public float CriticalDamageMultiplier => _criticalDamageMultiplier;
    public float PetAttackPower => _petAttackPower;
    public int PlayerLevel => _playerLevel;
    public int PetLevel => _petLevel;
    public int FailUpgrade => _failUpgrade;
    public int Round => _round;

    public float TotalDamage => _totalDamage;
    public int TotalUseCoupon => _totalUseCoupon;


    public void SaveState()
    {
        _initialStats = new float[]
        {
        _attackPower,
        _attackSpeed,
        _attackRange,
        _findRange,
        _moveSpeed,
        _rotateSpeed,
        _criticalChance,
        _criticalDamageMultiplier,
        _petAttackPower
        };

        _initialLevel = new int[]
        {
        _playerLevel,
        _petLevel,
        _failUpgrade,
        _round
        };
    }

    public void ResetState()
    {
        if (_initialStats == null || _initialLevel == null)
        {
            return;
        }
        _attackPower = _initialStats[0];
        _attackSpeed = _initialStats[1];
        _attackRange = _initialStats[2];
        _findRange = _initialStats[3];
        _moveSpeed = _initialStats[4];
        _rotateSpeed = _initialStats[5];
        _criticalChance = _initialStats[6];
        _criticalDamageMultiplier = _initialStats[7];
        _petAttackPower = _initialStats[8];

        _playerLevel = _initialLevel[0];
        _petLevel = _initialLevel[1];
        _failUpgrade = _initialLevel[2];
        _round = _initialLevel[3];

        _totalDamage = 0;
        _totalUseCoupon = 0;
    }

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

    public void AddRound(int value)
    {
        _round += value;
    }

    public void AddTotalCoupon(int value)
    {
        _totalUseCoupon += value;
    }

    public void AddTotalDamage(float value)
    {
        _totalDamage += value;
    }

}


