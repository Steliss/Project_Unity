using OpenCover.Framework.Model;
using UnityEngine;

public class PlayerReward : MonoBehaviour
{
    private PlayerData _playerData;

    public void Initialize(PlayerData playerData)
    {
        if (playerData == null)
        {
            Log.LogNull(nameof(PlayerReward), nameof(Initialize), nameof(playerData));
            return;
        }
        _playerData = playerData;
    }




}
