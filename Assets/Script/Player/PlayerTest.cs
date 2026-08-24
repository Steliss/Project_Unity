using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        // 회전 = 마우스, 이동 = wasd


        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * 10f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += transform.forward * -5f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += transform.right * 10f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * -10f * Time.deltaTime;
        }


        //_playerTransform = transform;
    }


}
