using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaterialChange : MonoBehaviour
{
    public enum RendererType
    {
        Clothes,
        Jacket,
        SwimSuit,
        Hair,
        GlassOn,
        GlassOff
    }

    // 인스펙터 관리
    [Serializable]
    private class RendererGroup
    {
        // 안경 따로 관리

        [SerializeField] private RendererType _suitType;

        [SerializeField] private List<SkinnedMeshRenderer> _playerRenderers;
        //[SerializeField] private List<SkinnedMeshRenderer> _uiPlayerRenderers;
        [SerializeField] private List<Material> _materials;

        public RendererType SuitType => _suitType;

        public List<SkinnedMeshRenderer> PlayerRenderers => _playerRenderers;
        //public List<SkinnedMeshRenderer> UIPlayerRenderers => _uiPlayerRenderers;
        public List<Material> Materials => _materials;

    }

    [SerializeField] private GameObject _player;
    //[SerializeField] private GameObject _uiPlayer;
    [SerializeField] private List<RendererGroup> _rendererGroups;

    private Transform _playerClothes;
    private Transform _playerjacket;
    private Transform _playerSwimSuit;
    private Transform _playerGlass;
    //private Transform _uiClothes;
    //private Transform _uijacket;
    //private Transform _uiSwimSuit;
    //private Transform _uiGlass;

    private GameData _gameData;
    private PlayerData _playerData;
    private ObjectData _objectData;

    private int _savePlayerLevel = -1;


    private void Awake()
    {
        if (_player == null) //  || _uiPlayer == null
        {
            Log.LogNull(nameof(PlayerUpgrade), nameof(Start));
        }

        _playerjacket = _player.transform.Find("Cloth/Outer");
        _playerClothes = _player.transform.Find("Cloth/Top");
        _playerSwimSuit = _player.transform.Find("Sportswear");
        _playerGlass = _player.transform.Find("root/pelvis/spine_01/spine_02/spine_03/neck_01");
        //_uijacket = _uiPlayer.transform.Find("Cloth/Outer");
        //_uiClothes = _uiPlayer.transform.Find("Cloth/Top");
        //_uiSwimSuit = _uiPlayer.transform.Find("Sportswear");
        //_uiGlass = _uiPlayer.transform.Find("root/pelvis/spine_01/spine_02/spine_03/neck_01");

        if (_playerjacket == null || _playerClothes == null || _playerSwimSuit == null) // || _uijacket == null || _uiClothes == null || _uiSwimSuit == null
        {
            Debug.Log("철자확인");
            return;
        }
    }

    private void Start()
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

    //의상 변경
    private void LateUpdate()
    {
        DetectLevelChange();
    }


    // 랜더 바꾸기
    private void ChangeMaterial(RendererType suitType, int materialIndex)
    {

        RendererGroup chooseSuit = null;
        foreach (RendererGroup render in _rendererGroups)
        {
            if (render != null && render.SuitType == suitType)
            {
                chooseSuit = render;
                break;
            }
        }

        if (chooseSuit == null)
        {
            Debug.LogWarning($"{suitType}을 찾지 못했습니다.");
            return;
        }
        if (chooseSuit.Materials == null || materialIndex < 0)
        {
            Debug.LogWarning($"{suitType}의 머티리얼 인덱스 {materialIndex}가 올바르지 않습니다.");
            return;
        }
        if (materialIndex >= chooseSuit.Materials.Count)
        {
            Debug.LogWarning($"{materialIndex} 초과로 0번으로 변경");
            materialIndex = 0;
        }

        Material selectedMaterial = chooseSuit.Materials[materialIndex];
        ClothesChange(suitType);
        ApplyMaterial(chooseSuit.PlayerRenderers, selectedMaterial);
        //ApplyMaterial(chooseSuit.UIPlayerRenderers, selectedMaterial);
    }

    // 랜더 적용
    private void ApplyMaterial(List<SkinnedMeshRenderer> renderers, Material material)
    {
        if (renderers == null || material == null)
        {
            return;
        }

        foreach (SkinnedMeshRenderer meshRenderer in renderers)
        {
            if (meshRenderer == null)
            {
                continue;
            }

            meshRenderer.sharedMaterial = material;
        }
    }

    // renderer Type에 따른 하이어라키 끄고 키기
    private void ClothesChange(RendererType suitType)
    {
        if (_rendererGroups == null)
        {
            return;
        }
        if (suitType == RendererType.Hair)
        {
            return;
        }

        switch (suitType)
        {
            case RendererType.Clothes:
                _playerjacket.gameObject.SetActive(false);
                _playerClothes.gameObject.SetActive(true);
                _playerSwimSuit.gameObject.SetActive(false);

               // _uijacket.gameObject.SetActive(false);
               // _uiClothes.gameObject.SetActive(true);
               // _uiSwimSuit.gameObject.SetActive(false);
                break;
            case RendererType.Jacket:
                _playerjacket.gameObject.SetActive(true);
                _playerClothes.gameObject.SetActive(false);
                _playerSwimSuit.gameObject.SetActive(false);

              //  _uijacket.gameObject.SetActive(true);
              //  _uiClothes.gameObject.SetActive(false);
              //  _uiSwimSuit.gameObject.SetActive(false);
                break;
            case RendererType.SwimSuit:
                _playerjacket.gameObject.SetActive(false);
                _playerClothes.gameObject.SetActive(false);
                _playerSwimSuit.gameObject.SetActive(true);

               // _uijacket.gameObject.SetActive(false);
               // _uiClothes.gameObject.SetActive(false);
               // _uiSwimSuit.gameObject.SetActive(true);
                break;

            case RendererType.GlassOn:
                _playerGlass.gameObject.SetActive(true);
              //  _uiGlass.gameObject.SetActive(true);
                break;

            case RendererType.GlassOff:
                _playerGlass.gameObject.SetActive(false);
               // _uiGlass.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }


    // 1~25 레벨에 따른 의상 변화
    private void DetectLevelChange()
    {
        if(_savePlayerLevel == _playerData.PlayerLevel)
        {
            return;
        }

        if(_playerData.PlayerLevel % 2 == 1)
        {
            ClothesChange(RendererType.GlassOn);
        }
        else
        {
            ClothesChange(RendererType.GlassOff);
        }

        if (_playerData.PlayerLevel == 0 || _playerData.PlayerLevel == 1)
        {
            ChangeMaterial(RendererType.SwimSuit, 0);
            ChangeMaterial(RendererType.Hair, 0);
        }
        else if (_playerData.PlayerLevel == 2 || _playerData.PlayerLevel == 3)
        {
            ChangeMaterial(RendererType.Clothes, 0);
            ChangeMaterial(RendererType.Hair, 0);
        }
        else if (_playerData.PlayerLevel == 4 || _playerData.PlayerLevel == 5)
        {
            ChangeMaterial(RendererType.Clothes, 1);
            ChangeMaterial(RendererType.Hair, 0);
        }
        else if (_playerData.PlayerLevel == 6 || _playerData.PlayerLevel == 7)
        {
            ChangeMaterial(RendererType.Clothes, 2);
            ChangeMaterial(RendererType.Hair, 1);
        }
        else if (_playerData.PlayerLevel == 8 || _playerData.PlayerLevel == 9)
        {
            ChangeMaterial(RendererType.Clothes, 3);
            ChangeMaterial(RendererType.Hair, 1);
        }
        else if (_playerData.PlayerLevel == 10 || _playerData.PlayerLevel == 11)
        {
            ChangeMaterial(RendererType.Clothes, 4);
            ChangeMaterial(RendererType.Hair, 2);
        }
        else if (_playerData.PlayerLevel == 12 || _playerData.PlayerLevel == 13)
        {
            ChangeMaterial(RendererType.Clothes, 5);
            ChangeMaterial(RendererType.Hair, 2);
        }
        else if (_playerData.PlayerLevel == 14 || _playerData.PlayerLevel == 15)
        {
            ChangeMaterial(RendererType.Jacket, 0);
            ChangeMaterial(RendererType.Hair, 3);
        }
        else if (_playerData.PlayerLevel == 16 || _playerData.PlayerLevel == 17)
        {
            ChangeMaterial(RendererType.Jacket, 1);
            ChangeMaterial(RendererType.Hair, 3);
        }
        else if (_playerData.PlayerLevel == 18 || _playerData.PlayerLevel == 19)
        {
            ChangeMaterial(RendererType.Jacket, 2);
            ChangeMaterial(RendererType.Hair, 4);
        }
        else if (_playerData.PlayerLevel == 20 || _playerData.PlayerLevel == 21)
        {
            ChangeMaterial(RendererType.Jacket, 3);
            ChangeMaterial(RendererType.Hair, 4);
        }
        else if (_playerData.PlayerLevel == 22 || _playerData.PlayerLevel == 23)
        {
            ChangeMaterial(RendererType.Jacket, 4);
            ChangeMaterial(RendererType.Hair, 5);
        }
        else if (_playerData.PlayerLevel == 24 || _playerData.PlayerLevel == 25)
        {
            ChangeMaterial(RendererType.Jacket, 1);
            ChangeMaterial(RendererType.Hair, 5);
        }

        _savePlayerLevel = _playerData.PlayerLevel;
    }



}