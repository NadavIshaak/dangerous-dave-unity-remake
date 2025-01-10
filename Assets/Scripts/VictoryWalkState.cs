using UnityEngine;

public class VictoryWalkState : PlayerState
{
    public VictoryWalkState(PlayerMovement player) : base(player) { }

    public override void Enter()
    {
        // Enter victory walk state logic
        player.SetTransform(player.GetVictoryWalkStart());
    }

    public override void HandleInput()
    {
        // No input handling during victory walk
    }

    public override void Update()
    {
        // Auto walk logic
        player.GetRigidbody().linearVelocity = new Vector2(player.GetMoveSpeed(), 0);
        player.GetAnimationConttroler().Move();

        // Raycast to detect walls
        RaycastHit2D hit = Physics2D.Raycast(player.GetTransform().position, Vector2.right, 0.5f, player.GetWallLayerMask());
        if (hit.collider != null)
        {
            player.TransitionToState(player.groundedState);
            player.TriggerVictoryWalkEnd();
            Object.Destroy(player.gameObject); // Delete the player
        }
    }

    public override void Exit()
    {
        // Exit victory walk state logic
    }
}