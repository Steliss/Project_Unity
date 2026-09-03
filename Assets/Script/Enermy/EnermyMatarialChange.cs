using System.Collections.Generic;
using UnityEngine;

public class EnemyMatarialChange : MonoBehaviour
{

    [SerializeField] private List<Material> _materials;

    public List<Material> Material => _materials;



    // 랜더 적용
    public void ApplyMaterial(int materialIndex, SkinnedMeshRenderer _skinnedMeshRenderer)
    {

        if (_skinnedMeshRenderer == null || _materials == null)
        {
            return;
        }
        Material selectedMaterial = _materials[materialIndex];
        _skinnedMeshRenderer.sharedMaterial = selectedMaterial;
    }
}
