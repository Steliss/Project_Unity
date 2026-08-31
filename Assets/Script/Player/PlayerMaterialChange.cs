using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaterialChange : MonoBehaviour
{
    // 수영복은 색으로 바꿔야함. 코드 찾아보기.
    // 옷 재킷 / 재킷없음 두개
    // 온오프까지 여기서 해결? ㅇㅋ

    public enum RendererType
    {
        Clothes,
        Jaket,
        SwimSuit,
        Hair
    }

    [Serializable]
    private class RendererGroup
    {
        [SerializeField] private RendererType _suitType;

        [SerializeField] private List<SkinnedMeshRenderer> _playerRenderers;
        [SerializeField] private List<SkinnedMeshRenderer> _uiPlayerRenderers;
        [SerializeField] private List<Material> _materials;


        public RendererType SuitType => _suitType;

        public List<SkinnedMeshRenderer> PlayerRenderers => _playerRenderers;
        public List<SkinnedMeshRenderer> UIPlayerRenderers => _uiPlayerRenderers;
        public List<Material> Materials => _materials;

    }

    [SerializeField] private List<RendererGroup> _rendererGroups;

    public void ChangeMaterial(RendererType suitType, int materialIndex)
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
        if (chooseSuit.Materials == null || materialIndex < 0 || materialIndex >= chooseSuit.Materials.Count)
        {
            Debug.LogWarning($"{suitType}의 머티리얼 인덱스 {materialIndex}가 올바르지 않습니다.");
            return;
        }


        Material selectedMaterial = chooseSuit.Materials[materialIndex];

        ApplyMaterial(chooseSuit.PlayerRenderers, selectedMaterial);
        ApplyMaterial(chooseSuit.UIPlayerRenderers, selectedMaterial);
    }

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
            Debug.Log($"test {meshRenderer.sharedMaterial}");
        }
    }


    private void Update()
    {
        // test
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeMaterial(RendererType.Jaket, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeMaterial(RendererType.Jaket, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeMaterial(RendererType.Jaket, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeMaterial(RendererType.Jaket, 3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeMaterial(RendererType.Jaket, 4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ChangeMaterial(RendererType.Jaket, 5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ChangeMaterial(RendererType.Jaket, 6);
        }
    }

}