using UnityEngine;

public class PlayerAnimationConttroler : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Move()
    {
        animator.SetTrigger("Move");
    }
    public void ChangeDirection(bool facingRight)
    {
        spriteRenderer.flipX = !facingRight;
    }
    public void StopInMovement()
    {
        animator.speed = 0;
    }
    public void ResumeMovement()
    {
        animator.speed = 1;
    }
    public void Jump()
    {
        animator.SetTrigger("Jump");
    }
    public void FallWhileWalking()
    {
        animator.SetTrigger("FallWhileWalking");
    }
    public void HitGroundWithMovement()
    {
        animator.SetTrigger("HitGroundWithMovement");
    }
    public void HitGroundWithoutMovement()
    {
        animator.SetTrigger("HitGroundWithoutMovement");
    }
}