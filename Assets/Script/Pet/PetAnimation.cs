using UnityEngine;

public class PetAnimation : MonoBehaviour
{

    [Header("참조")]
    [SerializeField] private Animator _animator;

    public enum Animation
    {
        IdleSmell, // Idle
        IdleYaw,   // Idle
        Confuse,   // Play
        PetVr,		// Play
        LevelUp,   // UI창
        Happy,	   // 승리
        Scared,	    // 패배
        FireAttack,		// 파이어볼
        PawR,		// 땅 공격1
        PawL,		// 땅 공격2
        Fly,	// 날기
        Run,		// 달리기
    }



    private static readonly int[] TriggerHashes =
    {
        Animator.StringToHash("tIdleSmell"),
        Animator.StringToHash("tIdleYaw"),
        Animator.StringToHash("tConfuse"),
        Animator.StringToHash("tPetVR"),
        Animator.StringToHash("tLevelUp"),
        Animator.StringToHash("tHappy"),
        Animator.StringToHash("tScared"),
        Animator.StringToHash("tFireBall"),
        Animator.StringToHash("tPawR"),
        Animator.StringToHash("tPawL")
    };

    private int _hashMove;

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

        _hashMove = Animator.StringToHash("fMove");
    }

    public void PlayAnimation(Animation animation)
    {
        if (_animator == null)
        {
            return;
        }

        int index = (int)animation;

        if (index < 0 || index >= TriggerHashes.Length)
        {
            return;
        }

        _animator.SetTrigger(TriggerHashes[index]);
    }

    /// <summary>
    /// 0 = idle / 0.5 = run / 1 = fly
    /// </summary>
    /// <param name="moveCondition"></param>
    public void PetMoving(float moveCondition)
    {
        _animator.SetFloat(_hashMove, moveCondition);
    }

}
