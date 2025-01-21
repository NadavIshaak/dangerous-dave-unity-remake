using UnityEngine;

public class AirborneState : PlayerState
{
    private readonly InputSystem_Actions _controls;
    private bool _hasJetPack;
    private bool _isOffGround;
    private readonly AudioClip _jumpSound;
    private bool _moveInAir;
    private bool _justTransitioned=true;
    private bool _isFalling=true;
    private readonly PlayerAnimationConttroler _animationConttroler;


    public AirborneState(PlayerMovement player) : base(player)
    {
        _controls = player.GetControls();
        _animationConttroler = player.GetAnimationConttroler();
        _jumpSound = player.GetJumpSound();
    }

    public override void Enter()
    {
        _isOffGround = false;
        _moveInAir = false;
        _justTransitioned = true;
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
            if (_moveInAir)
                player.GetAnimationConttroler().HitGroundWithMovement();
            else
                player.GetAnimationConttroler().HitGroundWithoutMovement();
            player.TransitionToState(player.GroundedState);
        }
    }

    private void CheckForNoFuel()
    {
        _hasJetPack = player.GetHasJetPack();
        if (_controls.Player.JetPack.WasPressedThisFrame() && _hasJetPack&&!_justTransitioned)
            if (JetPackState.GetCurrentFuel() > 0)
                player.TransitionToState(player.JetPackState);
        _justTransitioned = false;
    }
    
    

    private void InputAndAnimate()
    {
        if (_isFalling)
        {
            if(_justTransitioned)
                player.PlaySound(true, true, player.GetFallingSound());
            if (player.GetMoveInput().x != 0)
            {
                _animationConttroler.ResumeMovement();
                _animationConttroler.FallWhileWalking();
            }
        }
        else
        {
            if (_justTransitioned)
            {
                player.PlaySound(true, true, _jumpSound);
                _animationConttroler.Jump();
            }
        }
        CheckForNoFuel();
        player.MovePlayer();
        if (player.GetMoveInput().x != 0) _moveInAir = true;
        if (player.GetMoveInput().x > 0)
        {
            player.GetAnimationConttroler().ChangeDirection(true);
            player.SetIsRight(true);
        }
        else if (player.GetMoveInput().x < 0)
        {
            player.GetAnimationConttroler().ChangeDirection(false);
            player.SetIsRight(false);
        }
    }
    public override void Exit()
    {
        SoundManager.Instance.StopSound();
    }

    private bool IsGrounded()
    {
        // Get the player's collider bounds
        var collider = player.GetCollider();
        var bounds = collider.bounds;

        // Perform two raycasts to check if the player is grounded
        var bottomLeft = new Vector2(bounds.min.x, bounds.min.y + 0.1f); // Add a small buffer distance
        var bottomRight = new Vector2(bounds.max.x, bounds.min.y + 0.1f); // Add a small buffer distance

        var hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.145f, player.GetWallLayerMask());
        var hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.145f, player.GetWallLayerMask());
        switch (_isOffGround)
        {
            case false when hitLeft.collider is null || hitRight.collider is null:
                _isOffGround = true;
                return false;
            case false:
                return false;
            default:
                return hitLeft.collider is not null || hitRight.collider is not null;
        }
    }
    public void SetIsFalling(bool isFalling)
    {
        _isFalling = isFalling;
    }
}