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

    private bool isAirborne = false;
    private bool movedInAir = false;
    private bool isMovingOnGround = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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

                if (moveInput > 0) animator.SetTrigger("RightJump");
                else if (moveInput < 0) animator.SetTrigger("LeftJump");

                isMovingOnGround = false;
            }

            // If we move horizontally while airborne
            if (Mathf.Abs(moveInput) > 0.01f)
            {
                movedInAir = true;
            }
        }
        else
        {
            // On ground
            if (isAirborne)
            {
                // We just landed, do nothing => remain stuck in the last jump frame
                isAirborne = false;
                movedInAir = false;
                isMovingOnGround = false;
            }
            else
            {
                // Already on ground
                if (Mathf.Abs(moveInput) > 0.01f)
                {
                    // Trigger move only once at start
                    if (!isMovingOnGround)
                    {
                        if (moveInput > 0) animator.SetTrigger("RightMove");
                        else if (moveInput < 0) animator.SetTrigger("LeftMove");

                        isMovingOnGround = true;
                    }
                }
                else
                {
                    // Not moving => remain stuck in current animation
                    if (isMovingOnGround)
                    {
                        // If you want to remain stuck in the move frame, do nothing here
                        isMovingOnGround = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// If the movement script calls this directly on jump
    /// </summary>
    public void Jump(float moveInput)
    {
        isAirborne = true;
        movedInAir = false;
        if (isMovingOnGround)
        {
            if (moveInput > 0) animator.SetTrigger("RightJump");
            else if (moveInput < 0) animator.SetTrigger("LeftJump");
        }
    }
}