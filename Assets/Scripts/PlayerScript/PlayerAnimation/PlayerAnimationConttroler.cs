using UnityEngine;

public class PlayerAnimationConttroler : MonoBehaviour
{
    private static readonly int Move1 = Animator.StringToHash("Move");
    private static readonly int Jump1 = Animator.StringToHash("Jump");
    private static readonly int WhileWalking = Animator.StringToHash("FallWhileWalking");
    private static readonly int GroundWithMovement = Animator.StringToHash("HitGroundWithMovement");
    private static readonly int GroundWithoutMovement = Animator.StringToHash("HitGroundWithoutMovement");
    private static readonly int Death1 = Animator.StringToHash("Death");
    private static readonly int Pack = Animator.StringToHash("JetPack");
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
        animator.SetTrigger(Move1);
    }
    public void ChangeDirection(bool facingRight)
    {
        spriteRenderer.flipX = !facingRight;
    }
    public void StopInMovement()
    {
        animator.speed = 0;
    }
    public void StopMovement()
    {
        Invoke(nameof(StopInMovement), 0.175f);
    }
    
    public void ResumeMovement()
    {
        animator.speed = 1;
    }
    public void Jump()
    {
        animator.SetTrigger(Jump1);
    }
    public void FallWhileWalking()
    {
        animator.SetTrigger(WhileWalking);
    }
    public void HitGroundWithMovement()
    {
        animator.SetTrigger(GroundWithMovement);
    }
    public void HitGroundWithoutMovement()
    {
        animator.SetTrigger(GroundWithoutMovement);
    }
     public void Death()
    {
        animator.SetTrigger(Death1);
    }

    public void JetPack()
    {
        animator.SetTrigger(Pack);
    }
}