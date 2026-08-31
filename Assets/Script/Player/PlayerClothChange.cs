using System.Collections.Generic;
using UnityEngine;

public class PlayerClothChange : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _uiPlayer;

    private GameData _gameData;
    private PlayerData _playerData;
    private ObjectData _objectData;

    private int _previousLevel;

    void Start()
    {
        _gameData = ManagerDontDestroy.Instance.GameData;
        if (_gameData == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start), nameof(_gameData));
        }
        _playerData = _gameData._PlayerData;
        if (_playerData == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start), nameof(_playerData));
            return;
        }
        _objectData = _gameData._ObjectData;
        if (_objectData == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start), nameof(_objectData));
            return;
        }
    }

    void Update()
    {
        
    }


    // 레벨에 따른 상태변경 
    private void PlayerModelChange()
    {

        int currentLevel = _playerData.PlayerLevel;

        if (_previousLevel == currentLevel)
        {
            return;
        }

        _previousLevel = currentLevel;

        if (currentLevel <= 10)
        {

        }


        if (currentLevel > 10)
        {

        }




    }


}
