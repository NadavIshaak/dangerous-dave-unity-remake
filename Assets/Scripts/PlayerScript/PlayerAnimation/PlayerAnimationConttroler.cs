using UnityEngine;

/**
 * Player animation controller class that handles the player's animations
 */
public class PlayerAnimationConttroler : MonoBehaviour
{
    private static readonly int Move1 = Animator.StringToHash("Move");
    private static readonly int Jump1 = Animator.StringToHash("Jump");
    private static readonly int WhileWalking = Animator.StringToHash("FallWhileWalking");
    private static readonly int GroundWithMovement = Animator.StringToHash("HitGroundWithMovement");
    private static readonly int GroundWithoutMovement = Animator.StringToHash("HitGroundWithoutMovement");
    private static readonly int Death1 = Animator.StringToHash("Death");
    private static readonly int Pack = Animator.StringToHash("JetPack");
    private Animator _animator;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /**
     * on destroy stop all invokes
     */
    private void OnDestroy()
    {
        CancelInvoke();
    }

    /**
     * Play the move animation
     */
    public void Move()
    {
        _animator.SetTrigger(Move1);
        Debug.Log("Move animation");
    }

    /**
     * Change the direction of the player
     */
    public void ChangeDirection(bool facingRight)
    {
        _spriteRenderer.flipX = !facingRight;
    }

    /**
     * Stop the player's movement in the animation
     */
    public void StopInMovement()
    {
        _animator.speed = 0;
        Debug.Log("Stop animation");
    }

    /**
     * Stop the player's movement in the animation after a few frames
     */
    public void StopMovement()
    {
        Invoke(nameof(StopInMovement), 0.05f);
        Debug.Log("Invoke Stop");
    }

    /**
     * Resume the player's movement in the animation
     */
    public void ResumeMovement()
    {
        CancelInvoke();
        _animator.speed = 1;
        Debug.Log("Resume animation");
    }

    /**
     * Play the jump animation
     */
    public void Jump()
    {
        _animator.SetTrigger(Jump1);
    }

    /**
     * Play the fall animation while walking
     */
    public void FallWhileWalking()
    {
        _animator.SetTrigger(WhileWalking);
    }

    /**
     * Play the hit ground with movement animation
     */
    public void HitGroundWithMovement()
    {
        _animator.SetTrigger(GroundWithMovement);
    }

    /**
     * Play the hit ground without movement animation
     */
    public void HitGroundWithoutMovement()
    {
        _animator.SetTrigger(GroundWithoutMovement);
    }

    /**
     * Play the death animation
     */
    public void Death()
    {
        _animator.SetTrigger(Death1);
    }

    /**
     * Play the jetpack animation
     */
    public void JetPack()
    {
        _animator.SetTrigger(Pack);
    }
}