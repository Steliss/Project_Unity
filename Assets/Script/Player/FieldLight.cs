using UnityEngine;

public class FieldLight : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _forwardOffset = 2f;
    [SerializeField] private float _heightOffset = 1.5f;

    private void LateUpdate()
    {
        if (_player == null)
        {
            return;
        }

        transform.position = _player.position + _player.forward * _forwardOffset + Vector3.up * _heightOffset;
    }
}