using System;
using UnityEngine;

// 기타 오브젝트 관련 자료 공간 


[Serializable]
public class ObjectData
{
    // 상자 레벨, 보스순서, 레벨당 HP증가량?, 아이템에 대한 갯수, 

    [SerializeField] private int _chestLevel = 1;
    [SerializeField] private int _playerUpgrade = 0;
    [SerializeField] private int _PetUpgrade = 0;


    public int ChestLevel => _chestLevel;
    public int PlayerUpgrade => _playerUpgrade;

    public int PetUpgrade => _PetUpgrade;


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
