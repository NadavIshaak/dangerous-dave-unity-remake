using UnityEngine;

public class AirborneState : PlayerState
{
    private readonly InputSystem_Actions _controls;
    private readonly Collider2D _collider;
    private readonly AudioClip _fallingSound;
    private readonly LayerMask _wallLayerMask;
    private bool _hasJetPack;
    private bool _isOffGround;
    private readonly AudioClip _jumpSound;
    private bool _moveInAir;
    private bool _justTransitioned=true;
    private bool _isFalling=true;
    private bool _didMoveFromIdle;
    private readonly PlayerAnimationConttroler _animationConttroler;
    private readonly float _fallSpeed;
    private readonly float _airTime;
    private float _timeInAir;


    public AirborneState(PlayerMovement player) : base(player)
    {
        _controls = player.GetControls();
        _animationConttroler = player.GetAnimationConttroler();
        _jumpSound = player.GetJumpSound();
        _collider = player.GetCollider();
        _fallingSound = player.GetFallingSound();
        _wallLayerMask = player.GetWallLayerMask();
        _fallSpeed = player.GetFallSpeed();
        _airTime = player.GetAirTime();
    }

    public override void Enter()
    {
        _isOffGround = false;
        _moveInAir = false;
        _justTransitioned = true;
        _didMoveFromIdle = false;
        _timeInAir = -1;
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
               _animationConttroler.HitGroundWithMovement();
            else
              _animationConttroler.HitGroundWithoutMovement();
            Debug.Log("Grounded");
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
    private void ApplyConstantFall()
    {
        var velocity = player.GetRigidbody().linearVelocity;
        if (_timeInAir > 0&&!CheckForCollisionFromAbove())
        {
            velocity.y = -_fallSpeed;
            _timeInAir -= Time.deltaTime;
        }
        else
        {
            velocity.y = _fallSpeed;
            _timeInAir = -1;
        }
        if(velocity!=player.GetRigidbody().linearVelocity)
            player.GetRigidbody().linearVelocity = velocity;
    }

    private bool CheckForCollisionFromAbove()
    {
        var bounds = _collider.bounds;

        // Perform two raycasts to check if the player is grounded
        var topLeft = new Vector2(bounds.min.x, bounds.max.y); // Add a small buffer distance
        var topRight = new Vector2(bounds.max.x, bounds.max.y); // Add a small buffer distance

        var hitLeft = Physics2D.Raycast(topLeft, Vector2.up, 0.05f, _wallLayerMask);
        var hitRight = Physics2D.Raycast(topRight, Vector2.up, 0.05f, _wallLayerMask);
        return hitLeft.collider is not null || hitRight.collider is not null;
    }

    private void DidJump()
    {
        if (_justTransitioned)
        {
            player.PlaySound(true, true, _jumpSound);
            _animationConttroler.Jump();
            _timeInAir = _airTime;
        }
    }

    private void DidFall()
    {
        if(_justTransitioned)
            player.PlaySound(true, true, _fallingSound);
        if (player.GetMoveInput().x != 0&&!_didMoveFromIdle)
        {
            _animationConttroler.ResumeMovement();
            _animationConttroler.FallWhileWalking();
            _didMoveFromIdle = true;
        }
    }
    

    private void InputAndAnimate()
    {
        var moveInput = player.GetMoveInput();
        if (_isFalling)
        {
            DidFall();
        }
        else
        {
           DidJump();
        }
        ApplyConstantFall();
        CheckForNoFuel();
        player.MovePlayer();
        ChangeDirection(moveInput);
    }

    private void ChangeDirection(Vector2 moveInput)
    {
        switch (moveInput.x)
        {
            case > 0:
                _animationConttroler.ChangeDirection(true);
                player.SetIsRight(true);
                _moveInAir = true;
                break;
            case < 0:
                _animationConttroler.ChangeDirection(false);
                player.SetIsRight(false);
                _moveInAir = true;
                break;
        }
    }
    public override void Exit()
    {
        SoundManager.Instance.StopSound();
    }

    private bool IsGrounded()
    {
        // Get the player's collider bounds
        
        var bounds = _collider.bounds;

        // Perform two raycasts to check if the player is grounded
        var bottomLeft = new Vector2(bounds.min.x, bounds.min.y + 0.1f); // Add a small buffer distance
        var bottomRight = new Vector2(bounds.max.x, bounds.min.y + 0.1f); // Add a small buffer distance

        var hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.145f, _wallLayerMask);
        var hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.145f, _wallLayerMask);
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