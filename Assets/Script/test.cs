using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    private SaveData _saveData;
    private GameData GameData;
    private PlayerData PlayerData;
    private ObjectData ObjectData;

    void Start()
    {
        
        _saveData = ManagerDontDestroy.Instance.SaveData;
        GameData = ManagerDontDestroy.Instance.GameData;
        PlayerData = GameData._PlayerData;
        ObjectData = GameData._ObjectData;  
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _saveData.GetRelic();

            Debug.Log($"test : {_saveData}");
            Debug.Log($"ap : {PlayerData.AttackPower}");
            Debug.Log($"as : {PlayerData.AttackSpeed}");
            Debug.Log($"ms : {PlayerData.MoveSpeed}");
            Debug.Log($"rs : {PlayerData.RotateSpeed}");
            Debug.Log($"cc : {PlayerData.CriticalChance}");
            Debug.Log($"cm : {PlayerData.CriticalDamageMultiplier}");

        }

    }
}
