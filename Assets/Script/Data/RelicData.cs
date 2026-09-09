using System;
using UnityEngine;

public interface IRelic<T>
{
    T Valve { get; }
    void Apply(PlayerData playerData);
}


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
        relic.Apply(playerData);
    }

    private class Relic00 : IRelic<float>
    {
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

    private class Relic01 : IRelic<float>
    {
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

    private class Relic02 : IRelic<float>
    {
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

    private class Relic03 : IRelic<float>
    {
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

    private class Relic04 : IRelic<float>
    {
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

    private class Relic05 : IRelic<float>
    {
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