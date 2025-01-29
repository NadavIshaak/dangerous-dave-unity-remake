using UnityEngine;

/**
 * This class is responsible for handling the player's airborne state.
 */
public class AirborneState : PlayerState
{
    private const float ZeroVelocityFrames = 0.12f; // Number of frames with zero velocity
    private readonly float _airTime;
    private readonly PlayerAnimationConttroler _animationConttroler;
    private readonly Collider2D _collider;
    private readonly InputSystem_Actions _controls;
    private readonly AudioClip _fallingSound;
    private readonly float _fallSpeed;
    private readonly AudioClip _jumpSound;
    private readonly Rigidbody2D _rb;
    private readonly LayerMask _wallLayerMask;
    private float _currentZeroVelocityFrame;
    private bool _didMoveFromIdle;
    private bool _fromJetPack;
    private bool _hasJetPack;
    private bool _isFalling = true;
    private bool _justTransitioned = true;
    private bool _moveInAir;
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
        _rb = player.GetRigidbody();
    }

    /**
     * set the values for the default ones,
     * we did not move yet,
     * we just transitioned to this state,
     * we stay in air,
     * and not always jumping
     */
    public override void Enter()
    {
        _moveInAir = false;
        _justTransitioned = true;
        _didMoveFromIdle = false;
        _timeInAir = -1;
    }

    public override void HandleInput()
    {
    }

    /**
     * check if the player is grounded, if yes then transition to the grounded state,
     * if not check for player input
     */
    public override void Update()
    {
        // Airborne movement logic
        if (IfGrounded())
            if (!_justTransitioned)
            {
                return;
            }
            else
            {
                _justTransitioned = false;
                return;
            }

        InputAndAnimate();
    }

    /**
     * check if the player is grounded and if the player is grounded then transition to the grounded state
     * if the player is grounded then return true else return false,
     * change the animation to grounded animation according to the movement of the player
     */
    private bool IfGrounded()
    {
        if (IsGrounded() && !_justTransitioned)
        {
            if (_moveInAir)
                _animationConttroler.HitGroundWithMovement();

            else
                _animationConttroler.HitGroundWithoutMovement();
            _animationConttroler.ResumeMovement();
            player.TransitionToState(player.GroundedState);
            return true;
        }

        return false;
    }

    /**
     * check if the player has fuel and if the player has pressed the jetpack button and jetpack is enabled
     * if the player has fuel and has pressed the jetpack button then transition to the jetpack state
     * if the player has no fuel then do nothing
     */
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

    /**
     * Apply a constant fall speed to the player if they are in the air,
     * and not colliding with a wall from above,if the player is jumping
     * give him speed upwards
     */
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
        else if (_currentZeroVelocityFrame < ZeroVelocityFrames && !isCollidedFromAbove)
        {
            velocity.y = 0;
            _currentZeroVelocityFrame += Time.deltaTime;
        }
        else
        {
            velocity.y = _fallSpeed;
            _timeInAir = -1;
            _currentZeroVelocityFrame = ZeroVelocityFrames;
        }

        if (velocity != _rb.linearVelocity)
            _rb.linearVelocity = velocity;
    }

    /**
     * check if the player is colliding with a wall from above
     * if the player is colliding with a wall from above then return true else return false
     */
    private bool CheckForCollisionFromAbove()
    {
        var bounds = _collider.bounds;

        // Perform two raycasts to check if the player is grounded
        var topLeft = new Vector2(bounds.min.x + 0.02f, bounds.max.y); // Add a small buffer distance
        var topRight = new Vector2(bounds.max.x - 0.02f, bounds.max.y); // Add a small buffer distance

        var hitLeft = Physics2D.Raycast(topLeft, Vector2.up, 0.04f, _wallLayerMask);
        var hitRight = Physics2D.Raycast(topRight, Vector2.up, 0.04f, _wallLayerMask);
        return hitLeft.collider is not null || hitRight.collider is not null;
    }

    public override void FixedUpdate()
    {
        ApplyConstantFall();
    }

    /**
     * check if the player just transitioned to this state,if he did start jump movement
     * if not return
     */
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

    /**
     * check if the player just transitioned to this state,if he did start fall movement
     * if not check for movement  to set player animation up
     */
    private void DidFall()
    {
        if (_justTransitioned)
        {
            player.PlaySound(true, true, _fallingSound);
            if (!_fromJetPack)
                _animationConttroler.StopInMovement();
        }

        if (player.GetMoveInput().x != 0 && !_didMoveFromIdle)
        {
            _animationConttroler.FallWhileWalking();
            _animationConttroler.ResumeMovement();
            _didMoveFromIdle = true;
        }
    }

    /**
     * check if the player is falling or jumping and if the player is falling then call the fall function
     * if the player is jumping then call the jump function
     * if the player is not falling or jumping then check for input and animate the player
     */
    private void InputAndAnimate()
    {
        Debug.Log("Airborne");
        var moveInput = player.GetMoveInput();
        if (_isFalling)
            DidFall();
        else
            DidJump();

        if (CheckForNoFuel()) return;
        player.MovePlayer();
        ChangeDirection(moveInput);
    }

    /**
     * change the direction of the player according to the input of the player
     * if the player is moving right and the player is not facing right then change the direction to right
     * if the player is moving left and the player is not facing left then change the direction to left
     * set that we moved in the air.
     */
    private void ChangeDirection(Vector2 moveInput)
    {
        var isRight = player.GetIsRight();
        switch (moveInput.x)
        {
            case > 0:
                _moveInAir = true;
                if (isRight) return;
                _animationConttroler.ChangeDirection(true);
                player.SetIsRight(true);
                break;
            case < 0:
                _moveInAir = true;
                if (!isRight) return;
                _animationConttroler.ChangeDirection(false);
                player.SetIsRight(false);
                break;
        }
    }

    public override void Exit()
    {
        SoundManager.Instance.StopSound();
    }

    /**
     * check if the player is grounded,if the player is grounded then return true else return false
     * if we move up then we are not grounded
     */
    private bool IsGrounded()
    {
        // Get the player's collider bounds

        var bounds = _collider.bounds;
        var bufferSpace = -0.02f;
        if (_timeInAir > 0) return false;
        if (_rb.linearVelocity.y < 0) bufferSpace *= -1;
        else if (_currentZeroVelocityFrame < ZeroVelocityFrames || _rb.linearVelocity.x == 0) bufferSpace = 0;
        // Perform two raycasts to check if the player is grounded
        var bottomLeft = new Vector2(bounds.min.x + bufferSpace, bounds.min.y); // Add a small buffer distance
        var bottomRight = new Vector2(bounds.max.x - bufferSpace, bounds.min.y); // Add a small buffer distance
        var hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.06f, _wallLayerMask);
        var hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.06f, _wallLayerMask);
        return hitLeft.collider is not null || hitRight.collider is not null;
    }

    /**
     * set the value of the variable _isFalling to the value of the parameter isFalling
     */
    public void SetIsFalling(bool isFalling)
    {
        _isFalling = isFalling;
    }

    /**
     * set the value of the variable _fromJetPack to the value of the parameter fromJetPack
     */
    public void SetFromJetPack(bool fromJetPack)
    {
        _fromJetPack = fromJetPack;
    }
}