using UnityEngine;

public class PlayerAnimationConttroler : MonoBehaviour
{
    private Animator animator;
    private bool isRight = true; 
    private bool isMoving = false; // helps prevent repeatedly triggering "Move"

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Updates the player's animation based on horizontal input and vertical speed.
    /// This avoids re-triggering "Move" every frame when the player is moving.
    /// </summary>
    public void Move(float moveInput, float ySpeed)
    {
        // Update facing direction
        if (moveInput > 0)
        {
            if (!isRight)
            {
                isRight = true;
                animator.SetBool("IsRight", true);
            }

            if (ySpeed == 0)
            {
                if (!isMoving)
                {
                    animator.SetTrigger("Move");
                    isMoving = true;
                }
            }
            else
            {
                animator.SetTrigger("Jump");
                isMoving = false;
            }
        }
        else if (moveInput < 0)
        {
            if (isRight)
            {
                isRight = false;
                animator.SetBool("IsRight", false);
            }

            if (ySpeed == 0)
            {
                if (!isMoving)
                {
                    animator.SetTrigger("Move");
                    isMoving = true;
                }
            }
            else
            {
                animator.SetTrigger("Jump");
                isMoving = false;
            }
        }
        else
        {
            // No horizontal input
            if (ySpeed == 0)
            {
                // Only trigger "Stop" once when coming from "Move"
                if (isMoving)
                {
                    animator.SetTrigger("Stop");
                    isMoving = false;
                }
            }
            else
            {
                animator.SetTrigger("Jump");
                isMoving = false;
            }
        }
    }

    /// <summary>
    /// You can call this directly if you prefer a manual jump trigger.
    /// </summary>
    public void Jump()
    {
        animator.SetTrigger("Jump");
    }
}
