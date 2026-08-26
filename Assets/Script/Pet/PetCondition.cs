using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PetCondition : MonoBehaviour
{
    [SerializeField] private GameObject _player = null;
    
    // 펫 자체의 고유 데이터 관리 / 이속, 회전값, 회전반경, sin파 높이, 공격 속도, 순간이동 범위 

    // 펫자체 컨디션값(상태 행동값) 조정 = enum
    // 강화 값 => 플레이어 데이터 

    // 날아다님 플레이어를 둥글게 돌면서 위 아래로 움직임 
    // 가끔 착륙해서 행동
    // 공격할때는 목표를 바라보고 공격 
    // 땅/ 하늘의 모션 차이 있어야함 => 공격 방법이 달라짐 
    // 플레이어에 넣으려니 변수값을 바꿔야하네 흠..

    private PlayerBattle _playerBattle;
    private Enemy _enermy;

    // 개 빡쎌거 같은데... 

    private bool _flagIsFlying = false;

    private enum Condition
    {
        Idle,
        Move,
        Attack,
        Play
    }




    private void Awake()
    {
        if(_player == null)
        {
            Log.LogNull(nameof(PetCondition), nameof(Awake), nameof(_player));
        }

        if (_playerBattle == null)
        {
            _playerBattle = _player.GetComponent<PlayerBattle>();
        }
        if (_playerBattle == null)
        {
            Log.LogNull(nameof(PetCondition), nameof(Awake), nameof(_playerBattle));
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        _enermy = _playerBattle._Enemy;

        // Test
        if (_enermy != null)
        {
            Vector3 directionToEnemy = (_enermy.transform.position - transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 90f * Time.deltaTime);
        }



    }








}
