using Cinemachine;
using UnityEngine;

public class BossCameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _boss1;
    [SerializeField] private Transform _boss2;
    [SerializeField] private Transform _boss3;

    private CinemachineVirtualCamera _playerCamera;
    private CinemachineVirtualCamera _dollyCamera;
    private CinemachineSmoothPath _dollytrack;
    private CinemachineDollyCart _dollyCart;


    private void Awake()
    {
        TransformCheck();
        CameraSetting();

        PlayerCameraChange();
    }

    private void TransformCheck()
    {
        if(_player == null)
        {
            Log.LogNull(nameof(BossCameraController), nameof(TransformCheck), nameof(_player));
        }
        if (_boss1 == null)
        {
            Log.LogNull(nameof(BossCameraController), nameof(TransformCheck), nameof(_boss1));
        }
        if (_boss2 == null)
        {
            Log.LogNull(nameof(BossCameraController), nameof(TransformCheck), nameof(_boss2));
        }
        if (_boss3 == null)
        {
            Log.LogNull(nameof(BossCameraController), nameof(TransformCheck), nameof(_boss3));
        }
    }


    private void CameraSetting()
    {
        _playerCamera = transform.Find("PlayerCamera").GetComponent<CinemachineVirtualCamera>();
        _dollyCamera = transform.Find("DollyTrack/Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        _dollytrack = transform.Find("DollyTrack/Dolly Track").GetComponent<CinemachineSmoothPath>();
        _dollyCart = transform.Find("DollyTrack/Dolly Cart").GetComponent<CinemachineDollyCart>();

        _playerCamera.Follow = _player;
        _playerCamera.LookAt = _player;

        _dollyCamera.Follow = _dollyCart.transform;
    }

    public void DollyCameraChange(Transform target, Vector3 offSet)
    {

        if (target == null)
        {
            Log.LogNull(nameof(BossCameraController), nameof(DollyCameraChange));
            return;
        }

        _dollyCamera.LookAt = target;

        _dollytrack.transform.position = target.position;
        _dollytrack.transform.localScale = offSet;
        _dollytrack.InvalidateDistanceCache();

        _dollyCart.m_Position = 0f;
        _dollyCart.m_Speed = 0.1f;

        _playerCamera.Priority = 10;
        _dollyCamera.Priority = 20;
    }

    public void PlayerCameraChange()
    {
        _playerCamera.Follow = _player;
        _playerCamera.LookAt = _player;

        Vector3 offSet = new Vector3(0f, 8f, -12f);

        CinemachineTransposer transposer = _playerCamera.GetCinemachineComponent<CinemachineTransposer>();

        transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        transposer.m_FollowOffset = offSet;

        // 이전 카메라 상태의 영향을 초기화
        _playerCamera.PreviousStateIsValid = false;

        _playerCamera.Priority = 20;
        _dollyCamera.Priority = 10;
    }
}