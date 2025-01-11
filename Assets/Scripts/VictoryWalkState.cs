using UnityEngine;

public class VictoryWalkState : PlayerState
{
    public VictoryWalkState(PlayerMovement player) : base(player) { }
    private AudioClip WinSound;

    public override void Enter()
    {
        // Enter victory walk state logic
        player.SetTransform(player.GetVictoryWalkStart());
        WinSound=player.GetWinSound();
        SoundManager.Instance.PlaySound(WinSound,player.GetTransform(),1,false,false);
        player.GetAnimationConttroler().ChangeDirection(true);
         player.GetAnimationConttroler().Move();
    }

    public override void HandleInput()
    {
        // No input handling during victory walk
    }

    public override void Update()
    {
        // Auto walk logic
        player.GetRigidbody().linearVelocity = new Vector2(player.GetMoveSpeed()*1.1f, 0);
        // Raycast to detect walls
        RaycastHit2D hit = Physics2D.Raycast(player.GetTransform().position, Vector2.right, 0.5f, player.GetWallLayerMask());
        if (hit.collider != null)
        {
            player.TransitionToState(player.groundedState);
            player.TriggerVictoryWalkEnd();
            player.Invoke("DestroyPlayer", 0f);
        }
    }

    public override void Exit()
    {
        // Exit victory walk state logic
    }
}