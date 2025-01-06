using UnityEngine;

public class PlayerAnimationConttroler : MonoBehaviour
{
    private Animator animator;
    private bool isRight = true; 
    private bool isMoving = false;

    // Track whether we’re currently in the air
    private bool isAirborne = false;

    // Track whether we moved horizontally while in the air
    private bool movedInAir = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Updates the player's animation based on horizontal input (moveInput) and vertical speed (ySpeed).
    /// If the player did not move in the air, they'll land in a "middle" idle.
    /// If they did move, they'll land in a left/right idle.
    /// </summary>
    public void Move(float moveInput, float ySpeed)
    {
        // Determine facing direction
        if (moveInput > 0)
        {
            if (!isRight)
            {
                isRight = true;
                animator.SetBool("IsRight", true);
            }
        }
        else if (moveInput < 0)
        {
            if (isRight)
            {
                isRight = false;
                animator.SetBool("IsRight", false);
            }
        }

        // Jumping or falling?
        if (ySpeed != 0)
        {
            // We are airborne
            if (!isAirborne)
            {
                // Trigger jump once when leaving ground
                animator.SetTrigger("Jump");
                isAirborne = true;
                movedInAir = false; 
            }

            // If we press horizontal input while in the air
            if (moveInput != 0)
            {
                movedInAir = true;
            }
            isMoving = false;
        }
        else 
        {
            // ySpeed == 0 => on the ground
            if (isAirborne)
            {
                // We just landed
                isAirborne = false;
                if (movedInAir)
                {
                    // Land in left/right idle
                    if (isRight)
                    {
                        // Could be "StopRight" or "RightIdle" in your Animator
                        animator.SetTrigger("StopRight");
                    }
                    else
                    {
                        // Could be "StopLeft" or "LeftIdle"
                        animator.SetTrigger("StopLeft");
                    }
                }
                else
                {
                    // Land in a "middle" idle animation
                    animator.SetTrigger("StopMiddle");
                }
                movedInAir = false;
                isMoving = false;
            }
            else
            {
                // Already on the ground, check horizontal input
                if (moveInput != 0)
                {
                    // Move
                    if (!isMoving)
                    {
                        animator.SetTrigger("Move");
                        isMoving = true;
                    }
                }
                else
                {
                    // No horizontal input on ground => remain idle
                    if (isMoving)
                    {
                        // We just stopped
                        if (isRight)
                            animator.SetTrigger("StopRight");
                        else
                            animator.SetTrigger("StopLeft");
                        isMoving = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Optional direct call for jump if your code triggers it manually.
    /// </summary>
    public void Jump()
    {
        animator.SetTrigger("Jump");
        isAirborne = true;
        movedInAir = false;
    }
}