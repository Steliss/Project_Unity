using System;
using UnityEngine;

public interface IRelic<T>
{
    string Name { get; }
    string Description { get; }
    T Valve { get; }
    void Apply(PlayerData playerData);
}

[Serializable]
public class RelicData : MonoBehaviour
{
    private GameData gameData;
    private PlayerData playerData;

    public void DataSetting()
    {
        gameData = ManagerDontDestroy.Instance.GameData;
        playerData = gameData._PlayerData;
        if (playerData == null)
        {
            Log.LogNull(nameof(Boss), nameof(RelicData), nameof(playerData));
        }
    }


    public void SetRelic00(float valve)
    {
        if (valve < 0)
        {
            return;
        }

        IRelic<float> relic = new Relic00(valve);

        //Debug.Log($"test : {valve}");
        //Debug.Log($"test2 : {playerData}");

        relic.Apply(playerData);
    }

    public class Relic00 : IRelic<float>
    {
        public string Name => "힘의 유물";
        public string Description => $"공격력이 {Valve:F0} 증가합니다.";

        public float Valve { get; }

        public Relic00(float value)
        {
            Valve = value;
        }

        public void Apply(PlayerData playerData)
        {
            playerData.AddAttackPower(Valve);
        }
    }

    public void SetRelic01(float valve)
    {
        if (valve < 0)
        {
            return;
        }

        IRelic<float> relic = new Relic01(valve);
        relic.Apply(playerData);
    }

    public class Relic01 : IRelic<float>
    {
        public string Name => "공격속도의 유물";
        public string Description => $"공격속도가 {Valve:F0} 증가합니다.";

        public float Valve { get; }

        public Relic01(float value)
        {
            Valve = value;
        }

        public void Apply(PlayerData playerData)
        {
            playerData.AddAttackSpeed(Valve);
        }
    }

    public void SetRelic02(float valve)
    {
        if (valve < 0)
        {
            return;
        }

        IRelic<float> relic = new Relic02(valve);
        relic.Apply(playerData);
    }

    public class Relic02 : IRelic<float>
    {
        public string Name => "이동속도의 유물";
        public string Description => $"이속속도가 {Valve:F0} 증가합니다.";

        public float Valve { get; }

        public Relic02(float value)
        {
            Valve = value;
        }

        public void Apply(PlayerData playerData)
        {
            playerData.AddMoveSpeed(Valve);
        }
    }

    public void SetRelic03(float valve)
    {
        if (valve < 0)
        {
            return;
        }

        IRelic<float> relic = new Relic03(valve);
        relic.Apply(playerData);
    }

    public class Relic03 : IRelic<float>
    {
        public string Name => "회전속도의 유물";
        public string Description => $"회전속도가 {Valve:F0} 증가합니다.";

        public float Valve { get; }

        public Relic03(float value)
        {
            Valve = value;
        }

        public void Apply(PlayerData playerData)
        {
            playerData.AddRotateSpeed(Valve);
        }
    }

    public void SetRelic04(float valve)
    {
        if (valve < 0)
        {
            return;
        }

        IRelic<float> relic = new Relic04(valve);
        relic.Apply(playerData);
    }

    public class Relic04 : IRelic<float>
    {
        public string Name => "치명타확률의 유물";
        public string Description => $"치명타확률이 {Valve:F0} 증가합니다.";

        public float Valve { get; }

        public Relic04(float value)
        {
            Valve = value;
        }

        public void Apply(PlayerData playerData)
        {
            playerData.AddCriticalChance(Valve);
        }
    }

    public void SetRelic05(float valve)
    {
        if (valve < 0)
        {
            return;
        }

        IRelic<float> relic = new Relic05(valve);
        relic.Apply(playerData);
    }

    public class Relic05 : IRelic<float>
    {
        public string Name => "치명타 배율의 유물";
        public string Description => $"치명타 배율이 {Valve:F0} 증가합니다.";

        public float Valve { get; }

        public Relic05(float value)
        {
            Valve = value;
        }

        public void Apply(PlayerData playerData)
        {
            playerData.AddCriticalDamageMultiplier(Valve);
        }
    }
}