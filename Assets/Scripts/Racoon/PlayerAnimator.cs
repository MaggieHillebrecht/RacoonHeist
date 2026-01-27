using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    PlayerInputHandler input;

    void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetFloat("xVelocity", Mathf.Abs(input.MoveDir.x));
        animator.SetFloat("zVelocity", Mathf.Abs(input.MoveDir.z));
    }
}
