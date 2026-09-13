using UnityEngine;

public class KeySpine : MonoBehaviour
{
    [Header("착지")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _fallSpeed = 5f;
    [SerializeField] private float _groundOffset = 0.5f;
    [SerializeField] private float _rayDistance = 10f;

    [Header("회전")]
    [SerializeField] private float _rotateSpeed = 90f;

    private Vector3 _landingPosition;
    private bool _hasGround;
    private bool _isLanded;

    private void OnEnable()
    {
        _hasGround = false;
        _isLanded = false;
    }

    private void Update()
    {
        if (!_hasGround)
        {
            FindGround();

            if (!_hasGround)
            {
                return;
            }
        }

        if (!_isLanded)
        {
            transform.position = Vector3.MoveTowards(transform.position, _landingPosition, _fallSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _landingPosition) < 0.001f)
            {
                transform.position = _landingPosition;
                _isLanded = true;
            }

            return;
        }

        // 회전이 맘에 안드는데 오브젝트 자체의 문제라 넘어가기
        transform.Rotate(Vector3.left, _rotateSpeed * Time.deltaTime);
    }

    private void FindGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _rayDistance, _groundLayer, QueryTriggerInteraction.Ignore))
        {
            _landingPosition = hit.point + Vector3.up * _groundOffset;
            _hasGround = true;
        }
    }
}