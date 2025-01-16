public abstract class PlayerState
{
    protected PlayerMovement player;

    public PlayerState(PlayerMovement player)
    {
        this.player = player;
    }

    public abstract void Enter();
    public abstract void HandleInput();
    public abstract void Update();
    public abstract bool GetIsRight();
    public abstract void Exit();
}