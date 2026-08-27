using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // 에니메이터

    [Header("참조")]
    [SerializeField] private Animator _animator;

    enum Animation
    {
        None,
        tPutGun,
        tTakeGun,
        tReload,
        tStun,
        tDieF,
        tDieB
    }


    private int _hashSpeed;
    private int _hashShoot;

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        if (_animator == null)
        {
            Log.LogNull(nameof(PlayerAnimation), nameof(Awake));
        }
        _hashSpeed = Animator.StringToHash("fSpeed");
        _hashShoot = Animator.StringToHash("bShoot");
    }

    public void PlayerMoving(float stopDistance, float moveSpeed)
    {
        float speed01;

        if (stopDistance <= 0f)
        {
            // 목표 위치
            speed01 = 0f;
        }
        else if (stopDistance > moveSpeed)
        {
            // 남은 거리가 이동속도보다 큼
            speed01 = 1f;
        }
        else
        {
            // 목표 위치와 가까움
            speed01 = 0.5f;
        }

        _animator.SetFloat(_hashSpeed, speed01);
    }

    public void PlayerShoot(bool shoot)
    {
        _animator.SetBool(_hashShoot, shoot);
    }

}
