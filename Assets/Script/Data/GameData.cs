using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameData : MonoBehaviour
{

    private ObjectData _objectData;
    private PlayerData _playerData;

    public ObjectData _ObjectData => _objectData;
    public PlayerData _PlayerData => _playerData;

    private void Awake()
    {
        _playerData = new PlayerData();

        if (_playerData == null)
        {
            Log.LogNull(nameof(GameData), nameof(Awake), nameof(_playerData));
            return;
        }

        _objectData = new ObjectData();

        if (_objectData == null)
        {
            Log.LogNull(nameof(GameData), nameof(Awake), nameof(_objectData));
            return;
        }
    }
}
