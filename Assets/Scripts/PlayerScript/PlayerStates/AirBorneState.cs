using UnityEngine;

public class AirborneState : PlayerState
{
    private readonly float _airTime;
    private readonly PlayerAnimationConttroler _animationConttroler;
    private readonly Collider2D _collider;
    private readonly InputSystem_Actions _controls;
    private readonly AudioClip _fallingSound;
    private readonly float _fallSpeed;
    private readonly AudioClip _jumpSound;
    private readonly LayerMask _wallLayerMask;
    private readonly Rigidbody2D _rb;
    private bool _didMoveFromIdle;
    private bool _hasJetPack;
    private bool _isFalling = true;
    private bool _isOffGround;
    private bool _justTransitioned = true;
    private bool _moveInAir;
    private float _timeInAir;
    private readonly float _zeroVelocityFrames = 0.1f; // Number of frames with zero velocity
    private float _currentZeroVelocityFrame;


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
        _rb = player.GetRigidbody();
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
        if(IfGrounded()) return;
        InputAndAnimate();
    }

    private bool IfGrounded()
    {
        if (IsGrounded()&& !_justTransitioned)
        {
            if (_moveInAir)
                _animationConttroler.HitGroundWithMovement();
            
            else
                _animationConttroler.HitGroundWithoutMovement();
            Debug.Log("Grounded");
            _animationConttroler.ResumeMovement();
            player.TransitionToState(player.GroundedState);
            return true;
        }

        return false;
    }

    private bool CheckForNoFuel()
    {
        _hasJetPack = player.GetHasJetPack();
        if (_controls.Player.JetPack.WasPressedThisFrame() && _hasJetPack && !_justTransitioned)
            if (JetPackState.GetCurrentFuel() > 0)
            {
                player.TransitionToState(player.JetPackState);
                return true;
            }

        _justTransitioned = false;
        return false;
    }

    private void ApplyConstantFall()
    {
        var velocity = _rb.linearVelocity;
        var isCollidedFromAbove = CheckForCollisionFromAbove();
        if (_timeInAir > 0 && !isCollidedFromAbove)
        {
            velocity.y = -_fallSpeed;
            _timeInAir -= Time.deltaTime;
            _currentZeroVelocityFrame = 0; // Reset the zero velocity frame counter
        }
        else if (_currentZeroVelocityFrame < _zeroVelocityFrames&& !isCollidedFromAbove)
        {
            velocity.y = 0;
            _currentZeroVelocityFrame+=Time.deltaTime;
        }
        else
        {
            velocity.y = _fallSpeed;
            _timeInAir = -1;
            _currentZeroVelocityFrame = _zeroVelocityFrames;
        }

        if (velocity != _rb.linearVelocity)
            _rb.linearVelocity = velocity;
    }

    private bool CheckForCollisionFromAbove()
    {
        var bounds = _collider.bounds;

        // Perform two raycasts to check if the player is grounded
        var topLeft = new Vector2(bounds.min.x+0.02f, bounds.max.y); // Add a small buffer distance
        var topRight = new Vector2(bounds.max.x-0.02f, bounds.max.y); // Add a small buffer distance

        var hitLeft = Physics2D.Raycast(topLeft, Vector2.up, 0.04f, _wallLayerMask);
        var hitRight = Physics2D.Raycast(topRight, Vector2.up, 0.04f, _wallLayerMask);
        Debug.DrawRay(topLeft, Vector2.up * 0.02f, Color.red);
        Debug.DrawRay(topRight, Vector2.up * 0.02f, Color.red);
        return hitLeft.collider is not null || hitRight.collider is not null;
    }

    public override void FixedUpdate()
    {
        ApplyConstantFall();
    }

    private void DidJump()
    {
        if (_justTransitioned)
        {
            player.PlaySound(true, true, _jumpSound);
            _animationConttroler.Jump();
            _animationConttroler.ResumeMovement();
            _timeInAir = _airTime;
        }
    }

    private void DidFall()
    {
        if (_justTransitioned)
        {
            player.PlaySound(true, true, _fallingSound);
            _animationConttroler.StopInMovement();
        }

        if (player.GetMoveInput().x != 0 && !_didMoveFromIdle)
        {
            _animationConttroler.FallWhileWalking();
            _animationConttroler.ResumeMovement();
            _didMoveFromIdle = true;
        }
    }


    private void InputAndAnimate()
    {
        var moveInput = player.GetMoveInput();
        if (_isFalling)
            DidFall();
        else
            DidJump();
        
        if(CheckForNoFuel()) return;
        player.MovePlayer();
        ChangeDirection(moveInput);
    }

    private void ChangeDirection(Vector2 moveInput)
    {
        bool isRight = player.GetIsRight();
        switch (moveInput.x)
        {
            case > 0:
                _moveInAir = true;
                if(isRight) return;
                _animationConttroler.ChangeDirection(true);
                player.SetIsRight(true);
                break;
            case < 0:
                _moveInAir = true;
                if(!isRight) return;
                _animationConttroler.ChangeDirection(false);
                player.SetIsRight(false);
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
        var bufferSpace = -0.02f;
        if(_timeInAir>0) return false;
        if (_rb.linearVelocity.y < 0) bufferSpace *= -1;
        else if(_currentZeroVelocityFrame < _zeroVelocityFrames||_rb.linearVelocity.x==0) bufferSpace = 0;
        // Perform two raycasts to check if the player is grounded
        var bottomLeft = new Vector2(bounds.min.x+bufferSpace, bounds.min.y ); // Add a small buffer distance
        var bottomRight = new Vector2(bounds.max.x-bufferSpace, bounds.min.y); // Add a small buffer distance

        var hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.04f, _wallLayerMask);
        var hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.04f, _wallLayerMask);
        Debug.DrawRay(bottomLeft, Vector2.down * 0.02f, Color.red);
        Debug.DrawRay(bottomRight, Vector2.down * 0.02f, Color.red);
        Debug.Log("hit");
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