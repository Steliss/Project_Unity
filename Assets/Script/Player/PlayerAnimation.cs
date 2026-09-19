using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // 에니메이터

    [Header("참조")]
    [SerializeField] private Animator _animator;

    public enum Animation
    {
        tPutGun,
        tTakeGun,
        tReload,
        tStun,
        tDieF,
        tDieB,
        tGreeting,
        tSmile
    }

    private int _hashSpeed;
    private int _hashShoot;
    private readonly int[] TriggerHashes =
    {
        Animator.StringToHash("tPutGun"),
        Animator.StringToHash("tTakeGun"),
        Animator.StringToHash("tReload"),
        Animator.StringToHash("tStun"),
        Animator.StringToHash("tDieF"),
        Animator.StringToHash("tDieB"),
        Animator.StringToHash("tGreeting"),
        Animator.StringToHash("tSmile")
    };


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

    public void PlayerMoving(float moveSpeed)
    {
        _animator.SetFloat(_hashSpeed, moveSpeed);
    }

    public void PlayerShoot(bool shoot)
    {
        _animator.SetBool(_hashShoot, shoot);
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

}
