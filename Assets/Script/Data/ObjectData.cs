using System;
using Unity.VisualScripting;
using UnityEngine;

// 기타 오브젝트 관련 자료 공간 


[Serializable]
public class ObjectData
{
    [SerializeField] private int _chestLevel = 1;
    [SerializeField] private int _playerUpgrade = 0;
    [SerializeField] private int _PetUpgrade = 0;

    private int[] _initialLevel;

    public int ChestLevel => _chestLevel;
    public int PlayerUpgrade => _playerUpgrade;
    public int PetUpgrade => _PetUpgrade;


    public void SaveState()
    {
        _initialLevel = new int[]
        {
        _chestLevel,
        _playerUpgrade,
        _PetUpgrade
        };
    }

    public void ResetState()
    {
        if (_initialLevel == null)
        {
            return;
        }
        _chestLevel = _initialLevel[0];
        _playerUpgrade = _initialLevel[1];
        _PetUpgrade = _initialLevel[2];
    }




    public void AddChestLevel(int value)
    {
        _chestLevel += value;
    }

    public void AddPlayerUpgrade(int value)
    {
        // 애는 마이너스 값이 갈수도 있으니 앞에서 방어 코드 작성해주기.

        _playerUpgrade += value;
    }

    public void AddPetUpgrade(int value)
    {
        _PetUpgrade += value;
    }
}
