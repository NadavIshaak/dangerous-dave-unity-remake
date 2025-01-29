using UnityEngine;

/**
 * This class is responsible for handling the grounded state of the player.
 */
public class GroundedState : PlayerState
{
    private readonly PlayerAnimationConttroler _animationConttroler;
    private readonly Collider2D _collider;
    private readonly InputSystem_Actions _controls;
    private readonly float _jumpForce;
    private readonly AudioClip _moveSound;
    private readonly Transform _playerTransform;
    private readonly AudioClip _wallHitSound;
    private readonly LayerMask _wallLayerMask;
    private bool _firstMove;
    private bool _hasJetPack;
    private bool _isStop;
    private bool _isStuck;
    private bool _justTransitioned;
    private Vector2 _moveInput;

    public GroundedState(PlayerMovement player) : base(player)
    {
        _moveSound = player.GetMoveSound();
        _controls = player.GetControls();
        _animationConttroler = player.GetAnimationConttroler();
        _collider = player.GetCollider();
        _wallLayerMask = player.GetWallLayerMask();
        _wallHitSound = player.GetStuckSound();
    }

    /**
     * Set the player's first move to false.
     */
    public override void Enter()
    {
        _isStop = true;
        _firstMove = false;
        _justTransitioned = true;
    }


    public override void HandleInput()
    {
    }

    public override void Update()
    {
        CheckInputAndAnimate();
    }

    public override void Exit()
    {
        _animationConttroler.ResumeMovement();
    }

    /**
     * Transition to the airborne state.
     */
    private void JumpTransition()
    {
        player.AirborneState.SetIsFalling(false);
        player.TransitionToState(player.AirborneState);
    }

    /**
     * Transition to the airborne state.
     */
    private void FallTransition()
    {
        player.AirborneState.SetIsFalling(true);
        player.AirborneState.SetFromJetPack(false);
        player.TransitionToState(player.AirborneState);
    }

    /**
     * Check if the player is stuck on a wall. if yes play a sound and stop the player's movement.
     * If the player is not stuck, resume the player's movement. and stop the sound.
     */
    private bool IsStuck()
    {
        var bounds = _collider.bounds;
        var bottomLeft = new Vector2(bounds.min.x, bounds.min.y + 0.1f); // Add a small buffer distance
        var bottomRight = new Vector2(bounds.max.x, bounds.min.y + 0.1f); // Add a small buffer distance
        var topLeft = new Vector2(bounds.min.x, bounds.max.y - 0.1f);
        var topRight = new Vector2(bounds.max.x, bounds.max.y - 0.1f);
        var hitLeft = Physics2D.Raycast(bottomLeft, Vector2.left, 0.04f, _wallLayerMask);
        var hitRight = Physics2D.Raycast(bottomRight, Vector2.right, 0.04f, _wallLayerMask);
        var hitTopLeft = Physics2D.Raycast(topLeft, Vector2.left, 0.04f, _wallLayerMask);
        var hitTopRight = Physics2D.Raycast(topRight, Vector2.right, 0.04f, _wallLayerMask);
        if (((hitLeft.collider is not null || hitTopLeft.collider is not null) && player.GetMoveInput().x < 0)
            || ((hitRight.collider is not null || hitTopRight.collider is not null) && player.GetMoveInput().x > 0))
        {
            _animationConttroler.StopMovement();
            Debug.Log("Stuck");
            if (!_isStuck)
                player.PlaySound(true, true, _wallHitSound);
            _isStuck = true;
            return true;
        }

        if (!_isStuck) return false;
        SoundManager.Instance.StopSound();
        _animationConttroler.ResumeMovement();
        _isStuck = false;
        return false;
    }

    /**
     * If the player has a jetpack and the jetpack has fuel, and he presses the jetpack button,
     * transition to the jetpack state.
     * If the player has no fuel, do nothing.
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
     * Check if the player is falling, if yes transition to the fall state.
     */
    private bool CheckForFall()
    {
        var bounds = _collider.bounds;
        var buffer = 0.02f;
        var bottomLeft = new Vector2(bounds.min.x - buffer, bounds.min.y); // Add a small buffer distance
        var bottomRight = new Vector2(bounds.max.x + buffer, bounds.min.y); // Add a small buffer distance
        var hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.06f, _wallLayerMask);
        var hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.06f, _wallLayerMask);
        Debug.DrawRay(bottomLeft, Vector2.down * 0.06f, Color.red);
        Debug.DrawRay(bottomRight, Vector2.down * 0.06f, Color.red);
        if (hitLeft.collider is null && hitRight.collider is null)
        {
            FallTransition();
            return true;
        }

        return false;
    }

    /**
     * Check if the player pressed the jump button, if yes transition to the jump state.
     */
    private bool CheckForJump()
    {
        if (_controls.Player.Jump.triggered)
        {
            JumpTransition();
            return true;
        }

        return false;
    }

    /**
     * Check the player's input and animate the player accordingly.
     */
    private void CheckInputAndAnimate()
    {
        if (CheckForJump()) return;
        if (CheckForFall()) return;
        if (CheckForNoFuel()) return;
        player.MovePlayer();
        if (IsStuck()) return;
        if (player.GetMoveInput().x != 0) ChangeDirectionOfMovement();

        if (player.GetMoveInput().x == 0 && !_isStop && _firstMove)
            StopMovement();
        else if (player.GetMoveInput().x != 0 && _isStop && _firstMove) ContinueMovementAfterStop();
    }

    /**
     * Change the direction of the player's movement, and play the move sound.
     */
    private void ChangeDirectionOfMovement()
    {
        if (!_firstMove)
        {
            _animationConttroler.ResumeMovement();
            _animationConttroler.Move();
            player.PlaySound(true, true, _moveSound);
            Debug.Log("Move");
        }

        _firstMove = true;
        var isRight = player.GetIsRight();
        var moveInput = player.GetMoveInput().x;
        if ((moveInput > 0 && isRight) || (moveInput < 0 && !isRight)) return;
        _animationConttroler.ChangeDirection(moveInput > 0);
        player.SetIsRight(moveInput > 0);
    }

    /**
     * Stop the player's movement and stop the move sound.
     */
    private void StopMovement()
    {
        _animationConttroler.StopInMovement();
        SoundManager.Instance.StopSound();
        _isStop = true;
    }

    public override void FixedUpdate()
    {
        // No fixed update during grounded state
    }

    /**
     * Continue the player's movement after stopping it.
     */
    private void ContinueMovementAfterStop()
    {
        _animationConttroler.ResumeMovement();
        player.PlaySound(true, true, _moveSound);
        _isStop = false;
    }
}