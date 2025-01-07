using UnityEngine;

public class PlayerAnimationConttroler : MonoBehaviour
{
    private Animator animator;
    private bool isMoving = false;
    private float prevYSpeed;
    private bool isInAir = false;
    // Track whether we moved horizontally while in the air

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
        if(moveInput>0){
            animator.SetBool("IsRight", true);
            if(ySpeed==0&&!isInAir)
            animator.SetTrigger("Move");
            if(ySpeed==0&&prevYSpeed!=0)
            isInAir = false;
            isMoving = true;
        }
        else if(moveInput<0){
            animator.SetBool("IsRight", false);
            if(ySpeed==0&&!isInAir)
            animator.SetTrigger("Move");
            if(ySpeed==0&&prevYSpeed!=0)
            isInAir = false;
            isMoving = true;
        }
        else if(moveInput==0&&ySpeed==0&&!isInAir){
            animator.SetTrigger("Stop"); 
        }
        else if(moveInput==0&&ySpeed==0&&isInAir){
            isInAir = false;
            if(isMoving)
            animator.SetTrigger("StopInAir");
            else
            {
                animator.SetTrigger("StopMiddle");
            }
        }
    prevYSpeed=ySpeed;
    }

    /// <summary>
    /// Optional direct call for jump if your code triggers it manually.
    /// </summary>
    public void Jump()
    {
        animator.SetTrigger("Jump");
        isMoving = false;
        isInAir = true;
    }
}