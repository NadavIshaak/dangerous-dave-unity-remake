using UnityEngine;

public class DeathState : PlayerState
{
    public DeathState(PlayerMovement player) : base(player) { }

    public override void Enter()
    {
        // Trigger the death animation
        player.GetAnimationConttroler().Death();
        
        // Stop the player's movement
        player.GetRigidbody().linearVelocity = Vector2.zero;
        // Schedule the player to be destroyed after a few seconds
        player.Invoke("DestroyPlayer", 3f); // Adjust the delay as needed
    }

    public override void HandleInput()
    {
        // No input handling in death state
    }

    public override void Update()
    {
        // No update logic in death state
    }

    public override void Exit()
    {
    }
}