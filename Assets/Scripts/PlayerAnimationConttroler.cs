using UnityEngine;

public class PlayerAnimationConttroler : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;

    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip fallSound;

    private bool isAirborne = false;
    private bool movedInAir = false;
    private bool isMovingOnGround = false;
    private bool isRight = true; // Track the direction the player is facing

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Called every frame from PlayerMovement. 
    /// If ySpeed != 0, we handle air logic. Otherwise, we stay in a stuck 
    /// ground-move animation if the player is moving.
    /// </summary>
    public void Move(float moveInput, float ySpeed)
    {
        // If ySpeed != 0 => airborne
        if (Mathf.Abs(ySpeed) > 0.01f)
        {
            if (!isAirborne)
            {
                // Just jumped/fell
                isAirborne = true;
                movedInAir = false;

                if (isRight) animator.SetTrigger("RightJump");
                else animator.SetTrigger("LeftJump");

                isMovingOnGround = false;

                // Play jump sound
                PlaySound(jumpSound);
            }

            // If we move horizontally while airborne
            if (Mathf.Abs(moveInput) > 0.01f)
            {
                movedInAir = true;

                // Determine facing direction while airborne
                if (moveInput > 0 && !isRight)
                {
                    isRight = true;
                    animator.SetBool("IsRight", true);
                    animator.SetTrigger("RightJump");
                }
                else if (moveInput < 0 && isRight)
                {
                    isRight = false;
                    animator.SetBool("IsRight", false);
                    animator.SetTrigger("LeftJump");
                }
            }
        }
        else
        {
            // On ground
            if (isAirborne)
            {
                // We just landed
                isAirborne = false;
                HandleLanding();

                // Stop jump sound
                StopSound();
            }
            else
            {
                // Already on ground
                if (Mathf.Abs(moveInput) > 0.01f)
                {
                    // Determine facing direction
                    if (moveInput > 0 && !isRight)
                    {
                        isRight = true;
                        animator.SetBool("IsRight", true);
                        isMovingOnGround = false;
                    }
                    else if (moveInput < 0 && isRight)
                    {
                        isRight = false;
                        animator.SetBool("IsRight", false);
                        isMovingOnGround = false;
                    }

                    // Trigger move only once at start
                    if (!isMovingOnGround)
                    {
                        if (isRight) animator.SetTrigger("RightMove");
                        else animator.SetTrigger("LeftMove");

                        isMovingOnGround = true;
                        animator.speed = 1; // Ensure the animation is playing

                        // Play move sound
                        PlaySound(moveSound, true);
                    }
                }
                else
                {
                    // Not moving => remain stuck in current animation
                    if (isMovingOnGround)
                    {
                        // Stop the animation immediately
                        animator.speed = 0;
                        isMovingOnGround = false;

                        // Stop move sound
                       // StopSound();
                    }
                }
            }
        }
    }

    /// <summary>
    /// If the movement script calls this directly on jump
    /// </summary>
    public void Jump()
    {
        isAirborne = true;
        movedInAir = false;

        if (isRight)
        {
            animator.SetTrigger("RightJump");
        }
        else
        {
            animator.SetTrigger("LeftJump");
        }

        animator.speed = 1; // Ensure the jump animation is playing

        // Play jump sound
        PlaySound(jumpSound);
    }

    /// <summary>
    /// Handles the landing logic and triggers the appropriate idle animation.
    /// </summary>
    private void HandleLanding()
    {
        if (movedInAir)
        {
            if (isRight) animator.SetTrigger("RightIdle");
            else animator.SetTrigger("LeftIdle");
        }
        else
        {
            animator.SetTrigger("CenterIdle");
        }

        movedInAir = false;

        // Play fall sound if landing without jumping
        if (!movedInAir)
        {
            PlaySound(fallSound);
        }
    }

    /// <summary>
    /// Plays the specified sound.
    /// </summary>
    private void PlaySound(AudioClip clip,bool loop=false)
    {
        SoundManager.Instance.stopSound();
       SoundObject soundObject=SoundManager.Instance.PlaySound(clip, transform, 1,loop);
        SoundManager.Instance.setCurrentSoundObject(soundObject);
    }

    /// <summary>
    /// Stops the currently playing sound.
    /// </summary>
    private void StopSound()
    {
        SoundManager.Instance.stopSound();
    }
}