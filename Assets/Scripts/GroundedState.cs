using UnityEngine;

public class GroundedState : PlayerState
{
    public GroundedState(PlayerMovement player) : base(player) { }
    private bool isRight=false;
    private bool isStop=true;
    private bool isFalling=false;
    private bool firstMove=false;
    public override void Enter()
    {
        isStop=true;
        firstMove=false;
        Debug.Log("Enter grounded state");
    }

    public override void HandleInput()
    {
        if (player.GetControls().Player.Jump.triggered&&isFalling==false)
        {
            player.GetAnimationConttroler().Jump();
            player.GetRigidbody().AddForce(Vector2.up * player.GetJumpForce(), ForceMode2D.Impulse);
            player.TransitionToState(player.airborneState);
            Debug.Log("Transition to airborne state");
        }
    }

    public override void Update()
    {
        checkFirstMoveAndDirection();
        CheckInputAndAnimate();
    }

    public override void Exit()
    {
        player.GetAnimationConttroler().ResumeMovement();
    }

    private void checkFirstMoveAndDirection(){
        if(player.GetMoveInput().x!=0&&!firstMove)
        {
            player.GetAnimationConttroler().Move();
            if(player.GetMoveInput().x>0){
                player.GetAnimationConttroler().ChangeDirection(true);
                isRight=true;
            }
            else{
                player.GetAnimationConttroler().ChangeDirection(false);
                isRight=false;
            }
            firstMove=true;
        }
    }
    private void CheckInputAndAnimate(){
        Collider2D collider = player.GetCollider();
        Bounds bounds = collider.bounds;
         Vector2 bottomLeft = new Vector2(bounds.min.x, bounds.min.y + 0.1f); // Add a small buffer distance
        Vector2 bottomRight = new Vector2(bounds.max.x, bounds.min.y + 0.1f); // Add a small buffer distance
        RaycastHit2D hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.2f, player.GetWallLayerMask());
        RaycastHit2D hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.2f, player.GetWallLayerMask());
        player.GetRigidbody().linearVelocity = new Vector2(player.GetMoveInput().x * player.GetMoveSpeed(), player.GetRigidbody().linearVelocity.y);
        checkFirstMoveAndDirection();
        if(player.GetMoveInput().x>0&&!isRight&&!isFalling&&firstMove){
            player.GetAnimationConttroler().ChangeDirection(true);
            isRight=true;
        }
        else if(player.GetMoveInput().x<0&&isRight&&!isFalling){
            player.GetAnimationConttroler().ChangeDirection(false);
            isRight=false;
        }
        else if(player.GetMoveInput().x==0&&!isStop&&!isFalling&&firstMove){
            player.GetAnimationConttroler().StopInMovement();
            isStop=true;
        }
        else if(player.GetMoveInput().x!=0&&isStop&&!isFalling&&firstMove){
            player.GetAnimationConttroler().ResumeMovement();
            isStop=false;
        }
        else if(!isFalling&&(hitLeft.collider == null && hitRight.collider == null))
        {
            isFalling=true;
            player.GetAnimationConttroler().FallWhileWalking();
        }
        else if(isFalling&&(hitLeft.collider != null || hitRight.collider != null)){
            isFalling=false;
            isStop=true;
            firstMove=false;
            player.GetAnimationConttroler().HitGroundWithMovement();
        }
    }
}