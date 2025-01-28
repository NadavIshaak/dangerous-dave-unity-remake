using UnityEngine;
/**
 * This class is responsible for handling the player's death state.
 */
public class DeathState : PlayerState
{
    public DeathState(PlayerMovement player) : base(player)
    {
    }

    /** set the player to not be able to shoot,
     * trigger the death animation,
     * stop the player's movement,
     * and schedule the player to be destroyed after a few seconds
     */
    public override void Enter()
    {
        player.SetCanShoot(false);
        // Trigger the death animation
        player.GetAnimationConttroler().Death();

        // Stop the player's movement
        player.GetRigidbody().linearVelocity = Vector2.zero;
        // Schedule the player to be destroyed after a few seconds
        player.Invoke("DestroyPlayer", 3f); // Adjust the delay as needed
    }

    public override void FixedUpdate()
    {
        // No fixed update logic in death state
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