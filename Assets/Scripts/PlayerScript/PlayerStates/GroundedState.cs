using UnityEngine;

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

    private void JumpTransition()
    {
        player.AirborneState.SetIsFalling(false);
        player.TransitionToState(player.AirborneState);
    }

    private void FallTransition()
    {
        player.AirborneState.SetIsFalling(true);
        player.TransitionToState(player.AirborneState);
    }

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

    private void CheckForNoFuel()
    {
        _hasJetPack = player.GetHasJetPack();
        if (_controls.Player.JetPack.WasPressedThisFrame() && _hasJetPack && !_justTransitioned)
            if (JetPackState.GetCurrentFuel() > 0)
                player.TransitionToState(player.JetPackState);
        _justTransitioned = false;
    }

    private bool CheckForFall()
    {
        var bounds = _collider.bounds;
        var bottomLeft = new Vector2(bounds.min.x-0.02f, bounds.min.y ); // Add a small buffer distance
        var bottomRight = new Vector2(bounds.max.x+0.02f, bounds.min.y ); // Add a small buffer distance
        var hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.02f, _wallLayerMask);
        var hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.02f, _wallLayerMask);
        Debug.DrawRay(bottomLeft, Vector2.down * 0.02f, Color.red);
        Debug.DrawRay(bottomRight, Vector2.down * 0.02f, Color.red);
        if (hitLeft.collider is null && hitRight.collider is null)
        {
            Debug.Log("Fall transition");
            FallTransition();
            return true;
        }

        return false;
    }

    private bool CheckForJump()
    {
        if (_controls.Player.Jump.triggered)
        {
            JumpTransition();
            return true;
        }

        return false;
    }

    private void CheckInputAndAnimate()
    {
        if(CheckForFall()) return;
        if(CheckForJump()) return;
        CheckForNoFuel();
        player.MovePlayer();
        if (IsStuck()) return;
        if (player.GetMoveInput().x != 0) ChangeDirectionOfMovement();

        if (player.GetMoveInput().x == 0 && !_isStop && _firstMove)
        {
            StopMovement();
            Debug.Log("Stop");
        }
        else if (player.GetMoveInput().x != 0 && _isStop && _firstMove)
        {
            ContinueMovementAfterStop();
        }
    }

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
        bool isRight = player.GetIsRight();
        float moveInput = player.GetMoveInput().x;
        if (moveInput > 0 && isRight || moveInput < 0 && !isRight) return;
        _animationConttroler.ChangeDirection(moveInput > 0);
        player.SetIsRight(moveInput > 0);
    }

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

    private void ContinueMovementAfterStop()
    {
        Debug.Log("Continue");
        _animationConttroler.ResumeMovement();
        player.PlaySound(true, true, _moveSound);
        _isStop = false;
    }
}