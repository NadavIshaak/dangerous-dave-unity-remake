// using UnityEngine;

// public class PlayerAnimationConttroler : MonoBehaviour
// {
//     private Animator animator;
//     private bool isMoving = false;
//     private float prevYSpeed;
//     private bool isInAir = false;
//     // Track whether we moved horizontally while in the air

//     private void Awake()
//     {
//         animator = GetComponent<Animator>();
//     }

//     /// <summary>
//     /// Updates the player's animation based on horizontal input (moveInput) and vertical speed (ySpeed).
//     /// If the player did not move in the air, they'll land in a "middle" idle.
//     /// If they did move, they'll land in a left/right idle.
//     /// </summary>
//     public void Move(float moveInput, float ySpeed)
//     {
//         if(moveInput>0){
//             animator.SetBool("IsRight", true);
//             if(ySpeed==0&&!isInAir)
//             animator.SetTrigger("Move");
//             if(ySpeed==0&&prevYSpeed!=0)
//             isInAir = false;
//             isMoving = true;
//         }
//         else if(moveInput<0){
//             animator.SetBool("IsRight", false);
//             if(ySpeed==0&&!isInAir)
//             animator.SetTrigger("Move");
//             if(ySpeed==0&&prevYSpeed!=0)
//             isInAir = false;
//             isMoving = true;
//         }
//         else if(moveInput==0&&ySpeed==0&&!isInAir){
//             animator.SetTrigger("Stop"); 
//         }
//         else if(moveInput==0&&ySpeed==0&&isInAir){
//             isInAir = false;
//             if(isMoving)
//             animator.SetTrigger("StopInAir");
//             else
//             {
//                 animator.SetTrigger("StopMiddle");
//             }
//         }
//     prevYSpeed=ySpeed;
//     }

//     /// <summary>
//     /// Optional direct call for jump if your code triggers it manually.
//     /// </summary>
//     public void Jump()
//     {
//         animator.SetTrigger("Jump");
//         isMoving = false;
//         isInAir = true;
//     }
// }
using UnityEngine;

public class PlayerAnimationConttroler : MonoBehaviour
{
    private Animator animator;
    private bool isRight = true;
    private bool isAirborne = false;
    private bool movedInAir = false;

    // Track if we’re currently in a move state on the ground
    private bool isMovingOnGround = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Called every frame from PlayerMovement. 
    /// If the player didn’t move horizontally while in the air, they land in a “middle” idle. 
    /// If they did move, they land on a left/right idle.
    /// </summary>
    public void Move(float moveInput, float ySpeed)
    {
        // Check if we are in the air
        if (Mathf.Abs(ySpeed) > 0.01f)
        {
            // If we're not already airborne, trigger jump once
            if (!isAirborne)
            {
                isAirborne = true;
                movedInAir = false;

                // Trigger left/right jump
                if (isRight)
                    animator.SetTrigger("RightJump");
                else
                    animator.SetTrigger("LeftJump");

                isMovingOnGround = false;
            }

            // If we move while airborne, record it
            if (Mathf.Abs(moveInput) > 0.01f)
                movedInAir = true;
        }
        else
        {
            // We are on the ground
            if (isAirborne)
            {
                // We just landed
                isAirborne = false;

                // If we moved in air, land on left/right idle
                if (movedInAir)
                {
                    if (isRight)
                    {
                        animator.SetTrigger("RightIdle");
                    }
                    else
                    {
                        animator.SetTrigger("LeftIdle");
                    }
                }
                else
                {
                    // Land on middle idle
                    animator.SetTrigger("MiddleIdle");
                }

                movedInAir = false;
                isMovingOnGround = false;
            }
            else
            {
                // Already on the ground, handle movement
                if (Mathf.Abs(moveInput) > 0.01f)
                {
                    // Determine facing direction
                    if (moveInput > 0 && !isRight)
                    {
                        isRight = true;
                        // Switch to right if needed
                    }
                    else if (moveInput < 0 && isRight)
                    {
                        isRight = false;
                        // Switch to left if needed
                    }

                    // Trigger run/move animation only once when we start moving
                    if (!isMovingOnGround)
                    {
                        if (isRight)
                            animator.SetTrigger("RightMove");
                        else
                            animator.SetTrigger("LeftMove");

                        isMovingOnGround = true;
                    }
                }
                else
                {
                    // Not moving horizontally
                    if (isMovingOnGround)
                    {
                        // We just stopped moving
                        if (isRight)
                            animator.SetTrigger("RightIdle");
                        else
                            animator.SetTrigger("LeftIdle");

                        isMovingOnGround = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// If your PlayerMovement calls this directly on jump:
    /// </summary>
    public void Jump()
    {
        // Immediately set airborne & trigger jump anim
        isAirborne = true;
        movedInAir = false;

        if (isRight)
            animator.SetTrigger("RightJump");
        else
            animator.SetTrigger("LeftJump");
    }
}