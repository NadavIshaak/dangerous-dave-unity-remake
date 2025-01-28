// Purpose: Abstract class for player states.
public abstract class PlayerState
{
    protected PlayerMovement player;

    protected PlayerState(PlayerMovement player)
    {
        this.player = player;
    }

    public abstract void Enter();
    public abstract void HandleInput();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void Exit();
}