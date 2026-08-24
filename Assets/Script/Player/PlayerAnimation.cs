using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // 에니메이터
    // 들고 오는 김에 플레이어 위치 보정도? => 이동
    // 

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

    private 
    private float _speed;
    private Animation _anime = new Animation();

    public void PlayerMoving(float moveSpeed)
    {

    }

}
