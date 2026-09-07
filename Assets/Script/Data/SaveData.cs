using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum RelicType
{
    AttackPower,
    AttackSpeed,
    MoveSpeed,
    RotateSpeed,
    CriticalChance,
    CriticalDamageMultiplier,
}


[Serializable]
public class RelicOwnedData
{
    public RelicType Type;
    public int Level; // 0이면 미획득
}


[Serializable]
public class RelicSaveData
{
    public int Points = 0;
    public List<RelicOwnedData> Relics = new List<RelicOwnedData>();
}

public class SaveData : MonoBehaviour
{
    [Header("최대레벨")]
    [SerializeField] private int _maxAttackPower;
    [SerializeField] private int _maxAttackSpeed;
    [SerializeField] private int _maxMoveSpeed;
    [SerializeField] private int _maxRotateSpeed;
    [SerializeField] private int _maxCriticalChance;
    [SerializeField] private int _maxCriticalDamageMultiplier;

    [Header("레벨당 계수")]
    [SerializeField] private float _coeAttackPower;
    [SerializeField] private float _coeAttackSpeed;
    [SerializeField] private float _coeMoveSpeed;
    [SerializeField] private float _coeRotateSpeed;
    [SerializeField] private float _coeCriticalChance;
    [SerializeField] private float _coeCriticalDamageMultiplier;

    private RelicSaveData _saveData = new RelicSaveData
    {
        Relics = new List<RelicOwnedData>
        {
            new RelicOwnedData
            {
                Type = RelicType.AttackPower,
                Level = 0
            },
            new RelicOwnedData
            {
                Type = RelicType.AttackSpeed,
                Level = 0
            },
            new RelicOwnedData
            {
                Type = RelicType.MoveSpeed,
                Level = 0
            },
            new RelicOwnedData
            {
                Type = RelicType.RotateSpeed,
                Level = 0
            },
            new RelicOwnedData
            {
                Type = RelicType.CriticalChance,
                Level = 0
            },
            new RelicOwnedData
            {
                Type = RelicType.CriticalDamageMultiplier,
                Level = 0
            }
        }
    };

    private GameData gameData;
    private PlayerData _playerData;
    private ObjectData _objectData;

    private string SavePath => Path.Combine(Application.persistentDataPath, "relic.json");
    private RelicData _relicData;


    private void Start()
    {
        gameData = ManagerDontDestroy.Instance.GameData;
        if(gameData == null)
        {
            Log.LogNull(nameof(RelicSaveData), nameof(Start), nameof(gameData));
        }
        _playerData = gameData._PlayerData;
        _objectData = gameData._ObjectData;


        _relicData = ManagerDontDestroy.Instance.RelicData;
        _relicData.DataSetting();
        LoadRelic();

        _playerData.SaveState();
        _objectData.SaveState();
    }

    public void AddPoints(int valve)
    {
        // 소모처 만들때 데이터 저장 확인 
        _saveData.Points += valve;
        Debug.Log($"포인트 확인 : {_saveData.Points}");
    }



    public void Relic00()
    {
        foreach (RelicOwnedData relic in _saveData.Relics)
        {
            if (relic.Type == RelicType.AttackPower)
            {
                if (relic.Level < _maxAttackPower)
                {
                    relic.Level++;
                    break;
                }
                else
                {
                    AddPoints(100);
                    break;
                }
            }
        }
        SaveRelic();
    }

    public void Relic01()
    {
        foreach (RelicOwnedData relic in _saveData.Relics)
        {
            if (relic.Type == RelicType.AttackSpeed)
            {
                if (relic.Level < _maxAttackSpeed)
                {
                    relic.Level++;
                    break;
                }
                else
                {
                    AddPoints(100);
                    break;
                }
            }
        }
        SaveRelic();
    }

    public void Relic02()
    {
        foreach (RelicOwnedData relic in _saveData.Relics)
        {
            if (relic.Type == RelicType.MoveSpeed)
            {
                if (relic.Level < _maxMoveSpeed)
                {
                    relic.Level++;
                    break;
                }
                else
                {
                    AddPoints(100);
                    break;
                }
            }
        }
        SaveRelic();
    }

    public void Relic03()
    {
        foreach (RelicOwnedData relic in _saveData.Relics)
        {
            if (relic.Type == RelicType.RotateSpeed)
            {
                if (relic.Level < _maxRotateSpeed)
                {
                    relic.Level++;
                    break;
                }
                else
                {
                    AddPoints(100);
                    break;
                }
            }
        }
        SaveRelic();
    }

    public void Relic04()
    {
        foreach (RelicOwnedData relic in _saveData.Relics)
        {
            if (relic.Type == RelicType.CriticalChance)
            {
                if (relic.Level < _maxCriticalChance)
                {
                    relic.Level++;
                    break;
                }
                else
                {
                    AddPoints(100);
                    break;
                }
            }
        }
        SaveRelic();
    }

    public void Relic05()
    {
        foreach (RelicOwnedData relic in _saveData.Relics)
        {
            if (relic.Type == RelicType.CriticalDamageMultiplier)
            {
                if (relic.Level < _maxCriticalDamageMultiplier)
                {
                    relic.Level++;
                    break;
                }
                else
                {
                    AddPoints(100);
                    break;
                }
            }
        }
        SaveRelic();
    }



    private void SaveRelic()
    {
        string json = JsonUtility.ToJson(_saveData, true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"유물 저장 완료: {SavePath}");
    }


    private void LoadRelic()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("저장된 유물 파일이 없습니다.");
            return;
        }

        string json = File.ReadAllText(SavePath);

        RelicSaveData loadedData = JsonUtility.FromJson<RelicSaveData>(json);
        if (loadedData == null || loadedData.Relics == null)
        {
            Debug.LogWarning("유물 데이터가 올바르지 않습니다.");
            return;
        }

        _saveData = loadedData;

        // 초기 세팅. 인덱스 번호 주의
        RelicOwnedData relic = _saveData.Relics[0];
        if (relic.Type == RelicType.AttackPower && relic.Level != 0)
        {
            _relicData.SetRelic00(relic.Level * _coeAttackPower);
        }

        relic = _saveData.Relics[1];
        if (relic.Type == RelicType.AttackSpeed && relic.Level != 0)
        {
            _relicData.SetRelic01(relic.Level * _coeAttackSpeed);
        }

        relic = _saveData.Relics[2];
        if (relic.Type == RelicType.MoveSpeed && relic.Level != 0)
        {
            _relicData.SetRelic02(relic.Level * _coeMoveSpeed);
        }

        relic = _saveData.Relics[3];
        if (relic.Type == RelicType.RotateSpeed && relic.Level != 0)
        {
            _relicData.SetRelic03(relic.Level * _coeRotateSpeed);
        }

        relic = _saveData.Relics[4];
        if (relic.Type == RelicType.CriticalChance && relic.Level != 0)
        {
            _relicData.SetRelic04(relic.Level * _coeCriticalChance);
        }

        relic = _saveData.Relics[5];
        if (relic.Type == RelicType.CriticalDamageMultiplier && relic.Level != 0)
        {
            _relicData.SetRelic05(relic.Level * _coeCriticalDamageMultiplier);
        }

        SaveRelic();
    }
}