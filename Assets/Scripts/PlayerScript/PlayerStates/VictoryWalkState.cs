using UnityEngine;

/**
 * This class is responsible for handling the victory walk state of the player.
 */
public class VictoryWalkState : PlayerState
{
    private AudioClip WinSound;

    public VictoryWalkState(PlayerMovement player) : base(player)
    {
    }

    /**
     * set the player to not be able to shoot,
     * set the player's position to the start of the victory walk,
     * play the victory sound,
     * and start the victory walk animation
     */
    public override void Enter()
    {
        // Enter victory walk state logic
        player.SetCanShoot(false);
        player.SetTransform(player.GetVictoryWalkStart());
        WinSound = player.GetWinSound();
        SoundManager.Instance.PlaySound(WinSound, player.GetTransform(), 1);
        player.GetAnimationConttroler().ChangeDirection(true);
        player.GetAnimationConttroler().Move();
    }


    public override void HandleInput()
    {
        // No input handling during victory walk
    }

    public override void FixedUpdate()
    {
        // No fixed update during victory walk
    }

    /**
     * check if the player hit the wall, if yes destroy him and trigger the end of the victory walk
     * event.
     */
    public override void Update()
    {
        // Auto walk logic
        player.GetRigidbody().linearVelocity = new Vector2(player.GetMoveSpeed() * 1.2f, 0);
        // Raycast to detect walls
        var hit = Physics2D.Raycast(player.GetTransform().position, Vector2.right, 0.5f, player.GetWallLayerMask());
        if (hit.collider is null) return;
        player.TransitionToState(player.GroundedState);
        player.TriggerVictoryWalkEnd();
        player.Invoke("DestroyPlayer", 0f);
    }

    public override void Exit()
    {
        // Exit victory walk state logic
    }
}