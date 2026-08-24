using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;
using System.Collections.Generic;


// =========================================================
// Attribute : 필드나 클래스 등에 추가적인 정보를 부여하기 위해 사용
// =========================================================

// Inspector에서 ReadOnly로 표시할 필드임을 나타내는 Attribute
public class ReadOnlyAttribute : PropertyAttribute
{
}


// Inspector에서 필드의 표시 이름을 지정하기 위한 Attribute
public class RenameAttribute : PropertyAttribute
{
    public string NewName { get; private set; }

    public RenameAttribute(string name)
    {
        NewName = name;
    }
}


// =========================================================
// Socket Setting : AnimationEvent 명령을 연결하기 위한 정보를 저장하는 클래스
// =========================================================

// Unity가 해당 클래스의 필드를 직렬화하여 Inspector에서 관리할 수 있도록 함
[System.Serializable]
public class SocketSetting
{
    // 소켓의 이름 저장. ValidateWeaponSlots()에서 ParentConstraint의 Source 이름을 가져와 자동으로 넣음
    [ReadOnly]
    public string name;

    // AnimationEvent에서 Socket을 선택하기 위해 사용하는 명령 문자열
    [Rename("Animation Event String Name")]
    public string eventStringName;

    // 직렬화는 하지만 Inspector에는 표시하지 않겠다
    // ParentConstraint의 몇 번째 Source인지 여부 
    [HideInInspector]
    public int sourceIndex;
}


// =========================================================
// Weapon Slot // 무기의 소켓 전환을 하기 위해 필요한 정보를 한 묶음으로 관리하는 데이터 클래스
// =========================================================

[System.Serializable]
public class WeaponSlot
{
    // 무기 슬롯의 구분을 위한 이름 
    [Rename("Name & Debug")]
    public string name = "New Weapon";

    // 인스펙터 사이에 여백 부여 
    [Space(5)]

    // 실제 무기 소켓 전환의 핵심 참조
    [Rename("Parent Constraint Target")]
    public ParentConstraint weaponConstraint;


    [Space(5)]

    // ParentConstraint 연결을 끊기 위한 Animation Event 명령 문자열
    [Rename("Constraint Cut OFF Event String")]
    public string dropCommand = "Drop_Weapon";


    [Space(5)]

    // SocketSetting을 가지고 있는 리스트 (팔 등 허리)
    [Rename("Socket Settings(Animation Event)")]
    public List<SocketSetting> socketSettings = new List<SocketSetting>();
}


// =========================================================
// IK : IK를 어떤 Transform에, 어느 정도의 Weight로 적용할지 저장하는 설정 데이터 / IK는 포인트 끝
// =========================================================

[System.Serializable]
public class SupportIKConfig
{
    // 해당 IK 설정을 실제로 사용할지 결정하는 값
    [Rename("Use Support IK")]
    public bool useIK = false;

    // 어느 손이나 발의 IK를 제어할 것인지 지정하는 enum
    [Rename("IK Avatar Goal")]
    public AvatarIKGoal ikHand = AvatarIKGoal.LeftHand;

    // 손이 따라갈 목표 Transform
    [Rename("Target Transform")]
    public Transform ikTarget;

    // IK가 목표 Transform을 얼마나 강하게 따라갈지 결정하는 최대 Weight
    [Range(0f, 1f)]
    [Rename("IK Max Weight")]
    public float ikWeight = 1f;


    [Space(5)]
    // Animation Event에서 어떤 IK 설정을 제어할지 구분하기 위한 이름
    [Rename("IK Command ID")]
    [FormerlySerializedAs("commandID")]
    public string commandId = "CustomPart";

    // 실제로 Animator에 적용되고 있는 IK Weight
    [HideInInspector]
    public float currentWeight = 0f;

    // 현재 IK가 도달하려고 하는 목표 Weight
    [HideInInspector]
    public float targetWeight = 0f;
}


// =========================================================
// Prop : 애니메이션 이벤트를 통해 특정 GameObject를 켜거나 끄기 위한 설정 정보를 저장하는 클래스
// =========================================================

[System.Serializable]
public class PropEvent
{
    // 활성 대상
    public GameObject targetObject;

    // 활성화하기 위한 AnimationEvent 명령 문자열
    [Rename("Active ON String")]
    [FormerlySerializedAs("active_on")]
    public string activeOn;

    // 비활성화하기 위한 AnimationEvent 명령 문자열
    [Rename("Active OFF String")]
    [FormerlySerializedAs("active_off")]
    public string activeOff;
}


// =========================================================
// Character Weapon Controller
// =========================================================

public class Character_Weapon_Controller : MonoBehaviour
{
    #region 내부 변수
    // =====================================================
    // Event Prefix : Animation Event 명령 문자열의 공통 접두사를 한 곳에서 관리하기 위한 값 
    // 문자열 상수를 통한 변경 불가
    // =====================================================

    private const string SOCKET_PREFIX = "To_";
    private const string IK_ON_PREFIX = "IK_ON_";
    private const string IK_OFF_PREFIX = "IK_OFF_";
    private const string ACTIVE_ON_PREFIX = "Active_on_";
    private const string ACTIVE_OFF_PREFIX = "Active_off_";


    // =====================================================
    // Inspector : Inspector에서 사용자가 설정할 데이터를 모아둔 것
    // =====================================================

    [Space(5)]
    // 디버그 로그를 출력할지 말지를 결정하는 스위치
    [Rename("Show Debug Log")]
    public bool showDebugLog = false;


    [Space(10)]
    // 머신건 사용, 안사용여부 
    public List<WeaponSlot> weaponSlots = new List<WeaponSlot>();


    [Space(10)]
    // IK 설정들을 여러 개 등록하는 리스트 // 왼손 
    [Rename("IK Settings(Humanoid)")]
    public List<SupportIKConfig> supportIKSettings = new List<SupportIKConfig>();


    [Space(10)]
    // 활성화/비활성화할 Prop 설정들을 여러 개 등록하는 리스트 // 총에달린 탄창, 탄창
    [Rename("Prop Active Settings")]
    public List<PropEvent> propEvents = new List<PropEvent>();

    private Animator animator;
    #endregion

    // =====================================================
    // Awake : animator 저장
    // =====================================================

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    // =====================================================
    // OnValidate : Inspector 설정 변경을 바로 적용하기 위함. 해당 스크립트에선 Editor에서 값이 변경되거나 검증될 때 Socket과 Prop 설정을 자동 정리
    // =====================================================

    private void OnValidate()
    {
        // Editor에서 데이터를 정리하기 위한 기능으로 사용해서 플레이 중엔 리턴
        if (Application.isPlaying)
        {
            return;
        }

        // WeaponSlot / ParentConstraint / SocketSetting 정리
        ValidateWeaponSlots();
        // PropEvent의 명령 문자열 정리
        ValidatePropEvents();
    }


    // =====================================================
    // Weapon Slot Validate : WeaponSlot / ParentConstraint / SocketSetting 정리
    // =====================================================

    private void ValidateWeaponSlots()
    {
        // weaponSlots 리스트 자체가 존재하는지 검사
        if (weaponSlots == null)
        {
            return;
        }

        // weaponSlots에 등록된 모든 무기를 하나씩 확인
        foreach (WeaponSlot slot in weaponSlots)
        {
            // WeaponSlot은 존재하지만 ParentConstraint가 연결되지 않은 경우
            if (slot.weaponConstraint == null)
            {
                continue;
            }


            // Parent Constraint의 Source 개수와 SocketSetting List 크기 동기화
            // ParentConstraint.sourceCount는 현재 Constraint에 등록되어 있는 Source의 개수
            int sourceCount = slot.weaponConstraint.sourceCount;

            // SocketSetting이 부족한 경우 새 객체 생성
            while (slot.socketSettings.Count < sourceCount)
            {
                slot.socketSettings.Add(new SocketSetting());
            }

            // SocketSetting이 너무 많은 경우 객체 삭제
            while (slot.socketSettings.Count > sourceCount)
            {
                slot.socketSettings.RemoveAt(slot.socketSettings.Count - 1);
            }


            // ---------------------------------------------
            // Source 정보 저장
            // ---------------------------------------------

            for (int i = 0; i < sourceCount; i++)
            {
                ConstraintSource source = slot.weaponConstraint.GetSource(i);

                // Source에 Transform 연결 확인 후 있으면 오브젝트 이름, 비어 있으면 "None"
                string sourceName = source.sourceTransform != null ? source.sourceTransform.name : "None";

                // Source 개수와 socketSettings.Count를 맞췄음으로 인덱스로 활용
                SocketSetting setting = slot.socketSettings[i];

                setting.name = sourceName;
                setting.sourceIndex = i;


                // Event String이 비어있으면 자동 생성
                // 이벤트의 string과 parameter의 이름이 맞아야 정상 작동
                if (string.IsNullOrEmpty(setting.eventStringName))
                {
                    setting.eventStringName = SOCKET_PREFIX + sourceName;
                }
            }
        }
    }


    // =====================================================
    // Prop Validate : PropEvent의 명령 문자열 정리
    // =====================================================

    private void ValidatePropEvents()
    {
        if (propEvents == null)
        {
            return;
        }


        foreach (PropEvent prop in propEvents)
        {
            if (prop.targetObject == null)
            {
                prop.activeOn = "";
                prop.activeOff = "";

                continue;
            }

            // 비어있으면 자동 생성
            if (string.IsNullOrEmpty(prop.activeOn))
            {
                prop.activeOn = ACTIVE_ON_PREFIX + prop.targetObject.name;
            }


            if (string.IsNullOrEmpty(prop.activeOff))
            {
                prop.activeOff = ACTIVE_OFF_PREFIX + prop.targetObject.name;
            }
        }
    }


    // =====================================================
    // Animation Event : Animation Event에서 넘어온 문자열 명령과 애니메이션 클립 이름을 꺼내서, 실제 명령 처리 함수로 넘김
    // Animation Event에서 매서드 호출을 위해 열어둠
    // =====================================================

    public void SwitchSocket(AnimationEvent animEvent)
    {
        if (animEvent == null)
        {
            return;
        }

        // 실제 명령 문자열을 가져옴 
        string triggerName = animEvent.stringParameter;

        // 방어 코드 null값 대비 
        string clipName = "알 수 없는 애니메이션";

        // 실제 Animation Clip이 존재한다면 이름을 바꿈
        if (animEvent.animatorClipInfo.clip != null)
        {
            clipName = animEvent.animatorClipInfo.clip.name;
        }

        // 실제 명령 처리 흐름으로 전달 Prop -> IK -> Socket
        PerformSocketSwitch(triggerName, clipName);
    }


    // =====================================================
    // Script 직접 호출 : Animation Event가 아니라 다른 C# 스크립트에서 문자열 명령을 직접 실행하고 싶을 때 사용하는 진입점
    // =====================================================

    public void SwitchSocketByString(string triggerEventName)
    {
        PerformSocketSwitch(triggerEventName, "ScriptCall");
    }


    // =====================================================
    // Animation Event Command 분리 : 전달받은 하나의 문자열 안에 여러 명령이 들어 있을 경우 이를 분리해서 하나씩 처리
    // =====================================================

    private void PerformSocketSwitch(string triggerEventName, string sourceInfo)
    {
        if (string.IsNullOrWhiteSpace(triggerEventName))
        {
            return;
        }


        // 여러 명령 동시 실행 기능 유지. 예: To_MainHand,IK_ON_Mag
        string[] commands = triggerEventName.Split(',');


        foreach (string rawCommand in commands)
        {
            string command = rawCommand.Trim();


            if (string.IsNullOrEmpty(command))
            {
                continue;
            }

            // 하나의 명령을 Prop → IK → Weapon/Socket 중 어디에서 처리할지 분배
            ProcessSingleCommand(command, sourceInfo);
        }
    }


    // =====================================================
    // Command 분배 : 명령 하나를 받아서 어떤 종류의 명령인지 순서대로 판별하고, 실제 처리 함수로 넘기는 역할
    // =====================================================

    private void ProcessSingleCommand(string command, string sourceInfo)
    {
        // Prop비교
        if (TryProcessProp(command, sourceInfo))
        {
            return;
        }

        // IK 비교
        if (TryProcessIK(command, sourceInfo))
        {
            return;
        }

        // 소켓 비교 
        if (TryProcessWeapon(command, sourceInfo))
        {
            return;
        }

        // 로그 
        if (showDebugLog)
        {
            Debug.LogWarning($"[Command] " + $"등록되지 않은 명령 : " + $"{command}");
        }
    }


    // =====================================================
    // Prop : Prop의 ON/OFF 명령인지 확인 후 활성화 및 비활성화 
    // =====================================================

    private bool TryProcessProp(string command, string sourceInfo)
    {
        // Prop 설정 리스트가 없으면 처리하지 못하므로 false 반환
        if (propEvents == null)
        {
            return false;
        }


        foreach (PropEvent prop in propEvents)
        {
            if (prop.targetObject == null)
            {
                continue;
            }


            // 현재 들어온 값과 이 Prop의 ON 문자열이 같은지 확인
            if (IsSameCommand(command, prop.activeOn))
            {
                prop.targetObject.SetActive(true);

                if (showDebugLog)
                {
                    Debug.Log($"[Prop] " + $"'{sourceInfo}' -> " + $"'{prop.targetObject.name}' ON");
                }

                return true;
            }

            // 똑같음
            if (IsSameCommand(command, prop.activeOff))
            {
                prop.targetObject.SetActive(false);

                if (showDebugLog)
                {
                    Debug.Log($"[Prop] " + $"'{sourceInfo}' -> " + $"'{prop.targetObject.name}' OFF");
                }

                return true;
            }
        }

        // 리스트 끝까지 확인후 일치 Prop이 없을시 
        return false;
    }


    // =====================================================
    // IK : 손의 IK를 어떤 Transform에, 어느 정도의 Weight로 적용할지 저장하는 설정 데이터
    // targetWeight를 변경하고 실제 IK 위치/회전 적용은 OnAnimatorIK()에서 처리
    // =====================================================

    private bool TryProcessIK(string command, string sourceInfo)
    {
        if (supportIKSettings == null)
        {
            return false;
        }

        // 처리 상태 저장용 bool
        bool handled = false;


        foreach (SupportIKConfig ik in supportIKSettings)
        {
            if (string.IsNullOrEmpty(ik.commandId))
            {
                continue;
            }

            // 필요한 문자열 생성
            string offCommand = IK_OFF_PREFIX + ik.commandId;
            string onCommand = IK_ON_PREFIX + ik.commandId;

            // 문자열 비교 Off일때
            if (IsSameCommand(command, offCommand))
            {
                ik.targetWeight = 0f;

                if (showDebugLog)
                {
                    Debug.Log($"[SupportIK] " + $"'{sourceInfo}' -> " + $"'{ik.ikHand}' OFF");
                }

                handled = true;
            }

            // 문자열 비교 On일때
            else if (IsSameCommand(command, onCommand))
            {
                ik.targetWeight = ik.ikWeight;

                if (showDebugLog)
                {
                    Debug.Log($"[SupportIK] " + $"'{sourceInfo}' -> " + $"'{ik.ikHand}' ON");
                }

                handled = true;
            }
        }

        // 상태 반환
        return handled;
    }


    // =====================================================
    // Weapon / Socket : 실제 Socket 전환
    // =====================================================

    private bool TryProcessWeapon(string command, string sourceInfo)
    {
        if (weaponSlots == null)
        {
            return false;
        }

        // 처리 상태 저장용 bool
        bool handled = false;


        foreach (WeaponSlot slot in weaponSlots)
        {
            if (slot.weaponConstraint == null)
            {
                continue;
            }


            // =============================================
            // Drop
            // =============================================

            if (!string.IsNullOrEmpty(slot.dropCommand) && IsSameCommand(command, slot.dropCommand))
            {
                slot.weaponConstraint.constraintActive = false;


                // Drop 시 IK 전부 해제
                ReleaseAllIK();


                if (showDebugLog)
                {
                    Debug.Log($"[Weapon] " + $"'{sourceInfo}' -> " + $"'{slot.name}' DROP");
                }


                handled = true;


                continue;
            }

            // 소켓 검색 인덱스 
            int targetIndex = -1;


            foreach (SocketSetting setting in slot.socketSettings)
            {
                // 명령이름이 없는 Socket
                if (string.IsNullOrEmpty(setting.eventStringName))
                {
                    continue;
                }

                // Animation Event 명령과 SocketSetting의 문자열 확인
                if (IsSameCommand(setting.eventStringName.Trim(), command))
                {
                    // ValidateWeaponSlots()에서 저장해둔 ParentConstraint Source 인덱스를 가져옴
                    targetIndex = setting.sourceIndex;
                    break;
                }
            }

            if (targetIndex == -1)
            {
                continue;
            }

            // Constraint 껴졌을때를 대비 다시 활성화
            slot.weaponConstraint.constraintActive = true;

            // ParentConstraint의 Source 개수를 가져옴
            int sourceCount = slot.weaponConstraint.sourceCount;


            for (int i = 0; i < sourceCount; i++)
            {
                ConstraintSource source = slot.weaponConstraint.GetSource(i);

                // 선택된 Socket만 Weight 1, 나머지는 0
                source.weight = i == targetIndex ? 1f : 0f;

                // GetSource(i)로 가져온 ConstraintSource는 값을 수정한 뒤 다시 SetSource()로 ParentConstraint에 반영
                slot.weaponConstraint.SetSource(i, source);
            }


            if (showDebugLog)
            {
                Debug.Log($"[Socket] " + $"'{sourceInfo}' -> " + $"'{slot.name}' " + $"Command={command}, " + $"Index={targetIndex}");
            }


            handled = true;
        }

        return handled;
    }


    // =====================================================
    // 모든 IK 해제 : 현재 등록된 모든 IK의 목표 Weight를 0으로 만듬
    // =====================================================

    private void ReleaseAllIK()
    {
        if (supportIKSettings == null)
        {
            return;
        }


        foreach (SupportIKConfig ik in supportIKSettings)
        {
            ik.targetWeight = 0f;
        }
    }


    // =====================================================
    // Command 문자열 비교 : 두 명령 문자열이 같은지 비교 대소문자 무시
    // 기존 코드는 같은 함수의 반복으로 새로 만들어줌
    // =====================================================

    private bool IsSameCommand(string commandA, string commandB)
    {
        return string.Equals(commandA, commandB, System.StringComparison.OrdinalIgnoreCase);
    }


    // =====================================================
    // Animator IK : 목표 IK까지의 이동
    // =====================================================

    // OnAnimatorIK : Unity가 Animator의 IK를 계산하는 시점에 호출하는 생명주기 메서드
    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || supportIKSettings == null)
        {
            return;
        }


        foreach (SupportIKConfig ik in supportIKSettings)
        {
            // 사용할 IK나 목표가 없음
            if (!ik.useIK || ik.ikTarget == null)
            {
                continue;
            }

            // IK 보간을 통한 자연스러운 움직임 구현 
            ik.currentWeight = Mathf.Lerp(ik.currentWeight, ik.targetWeight, Time.deltaTime * 15f);

            // 해당 손이 IK 위치(Position)에 얼마나 영향을 줄지
            animator.SetIKPositionWeight(ik.ikHand, ik.currentWeight);

            // 회전(Rotation)의 IK 영향도
            animator.SetIKRotationWeight(ik.ikHand, ik.currentWeight);

            // 실제 IK의 목표 위치를 지정 ikTarget 포지션으로 지정
            animator.SetIKPosition(ik.ikHand, ik.ikTarget.position);

            // 목표 회전도 ikTarget의 회전으로 설정 
            animator.SetIKRotation(ik.ikHand, ik.ikTarget.rotation);
        }
    }
}