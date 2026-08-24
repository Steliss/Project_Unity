using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // 에니메이터
    // 들고 오는 김에 플레이어 위치 보정도? => 이동
    // 

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


    private Animation _anime = new Animation();

    private int _hashSpeed;

    private void Awake()
    {
        _hashSpeed = Animator.StringToHash("fSpeed");
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

}
