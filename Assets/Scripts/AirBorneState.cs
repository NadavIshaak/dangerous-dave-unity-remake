using UnityEngine;

public class AirborneState : PlayerState
{
    bool isOffGround=false;
    bool moveInAir=false;
    public AirborneState(PlayerMovement player) : base(player) { }

    public override void Enter()
    {
        isOffGround=false;
        moveInAir=false;
    }

    public override void HandleInput()
    {
    }

    public override void Update()
    {
        // Airborne movement logic
        IfGrounded();
        InputAndAnimate();
    }
    private void IfGrounded()
    {
        if (IsGrounded())
        {
            if(moveInAir){
                player.GetAnimationConttroler().HitGroundWithMovement();
            }
            else{
                player.GetAnimationConttroler().HitGroundWithoutMovement();
            }
            player.TransitionToState(player.groundedState);
            
        }
    }
    private void InputAndAnimate()
    {
        player.GetRigidbody().linearVelocity = new Vector2(player.GetMoveInput().x * player.GetMoveSpeed(), player.GetRigidbody().linearVelocity.y);
        if(player.GetMoveInput().x!=0){
            moveInAir=true;
        }
        if(player.GetMoveInput().x>0){
            player.GetAnimationConttroler().ChangeDirection(true);
        }
        else if(player.GetMoveInput().x<0){
            player.GetAnimationConttroler().ChangeDirection(false);
        }
    }

    public override void Exit()
    {
        // Exit airborne state logic
    }
    private bool IsGrounded()
    {
        // Get the player's collider bounds
        Collider2D collider = player.GetCollider();
        Bounds bounds = collider.bounds;

        // Perform two raycasts to check if the player is grounded
         Vector2 bottomLeft = new Vector2(bounds.min.x, bounds.min.y + 0.1f); // Add a small buffer distance
        Vector2 bottomRight = new Vector2(bounds.max.x, bounds.min.y + 0.1f); // Add a small buffer distance

        RaycastHit2D hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.12f, player.GetWallLayerMask());
        RaycastHit2D hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.12f, player.GetWallLayerMask());
        if(!isOffGround&&(hitLeft.collider == null || hitRight.collider == null))
        {
            isOffGround=true;
        }
        Debug.DrawRay(bottomLeft, Vector2.down * 0.12f, Color.red);
        Debug.DrawRay(bottomRight, Vector2.down * 0.12f, Color.red);
        if(isOffGround==false){
            return false;
        }
        return hitLeft.collider != null || hitRight.collider != null;
    }
}