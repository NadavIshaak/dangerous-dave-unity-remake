using UnityEngine;

public class GroundedState : PlayerState
{
    private readonly PlayerAnimationConttroler _animationConttroler;
    private readonly Collider2D _collider;
    private readonly InputSystem_Actions _controls;
    private readonly AudioClip _fallingSound;
    private readonly float _jumpForce;
    private readonly AudioClip _moveSound;
    private readonly float _moveSpeed;
    private readonly Transform _playerTransform;
    private readonly Rigidbody2D _rb;
    private readonly AudioClip _wallHitSound;
    private readonly LayerMask _wallLayerMask;
    private bool _firstMove;
    private bool _hasJetPack;
    private bool _isFalling;
    private bool _isRight;
    private bool _isStop;
    private bool _isStuck;
    private bool _justTransitioned;
    private Vector2 _moveInput;

    public GroundedState(PlayerMovement player) : base(player)
    {
        _moveSound = player.GetMoveSound();
        _fallingSound = player.GetFallingSound();
        _controls = player.GetControls();
        _animationConttroler = player.GetAnimationConttroler();
        _playerTransform = player.GetTransform();
        _jumpForce = player.GetJumpForce();
        _collider = player.GetCollider();
        _rb = player.GetRigidbody();
        _wallLayerMask = player.GetWallLayerMask();
        _moveSpeed = player.GetMoveSpeed();
        _wallHitSound = player.GetStuckSound();
    }

    public override void Enter()
    {
        _isStop = true;
        _firstMove = false;
        _hasJetPack = player.GetHasJetPack();
        _justTransitioned = true;
    }


    public override void HandleInput()
    {
        if (!_controls.Player.Jump.triggered || _isFalling) return;
        JumpTransition();
    }

    public override bool GetIsRight()
    {
        return _isRight;
    }

    public override void Update()
    {
        CheckFirstMoveAndDirection();
        CheckInputAndAnimate();
    }

    public override void Exit()
    {
        _animationConttroler.ResumeMovement();
    }

    private void PlaySound(bool shouldKeep, bool shouldLoop, AudioClip clip)
    {
        SoundManager.Instance.PlaySound(clip, _playerTransform, 1, shouldLoop, shouldKeep);
    } // ReSharper disable Unity.PerformanceAnalysis

    // ReSharper disable Unity.PerformanceAnalysis
    private void CheckFirstMoveAndDirection()
    {
        if (player.GetMoveInput().x != 0 && !_firstMove)
        {
            _animationConttroler.Move();
            Debug.Log("first Move");
            PlaySound(true, true, _moveSound);
            if (player.GetMoveInput().x > 0)
            {
                _animationConttroler.ChangeDirection(true);
                _isRight = true;
            }
            else
            {
                _animationConttroler.ChangeDirection(false);
                _isRight = false;
            }

            _firstMove = true;
        }
        else if (_controls.Player.Jump.triggered && !_firstMove)
        {
            _firstMove = true;
            JumpTransition();
        }

        IsStuck();
    }

    private void JumpTransition()
    {
        _animationConttroler.Jump();
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
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
        if ((hitLeft.collider is not null || hitTopLeft.collider is not null) && player.GetMoveInput().x < 0)
        {
            _animationConttroler.StopMovement();
            Debug.Log("Stuck");
            if (!_isStuck)
                PlaySound(true, true, _wallHitSound);
            _isStuck = true;
            return true;
        }

        if ((hitRight.collider is not null || hitTopRight.collider is not null) && player.GetMoveInput().x > 0)
        {
            Debug.Log("Stuck");
            _animationConttroler.StopMovement();
            if (!_isStuck)
                PlaySound(true, true, _wallHitSound);
            _isStuck = true;
            return true;
        }

        if (!_isStuck) return false;
        SoundManager.Instance.StopSound();
        _isStuck = false;
        return false;
    }

    private void CheckInputAndAnimate()
    {
        if (_controls.Player.JetPack.WasPressedThisFrame() && _hasJetPack&&!_justTransitioned)
            if (JetPackState.GetCurrentFuel() > 0)
                player.TransitionToState(player.JetPackState);
        _justTransitioned = false;

        var bounds = _collider.bounds;
        var bottomLeft = new Vector2(bounds.min.x, bounds.min.y + 0.1f); // Add a small buffer distance
        var bottomRight = new Vector2(bounds.max.x, bounds.min.y + 0.1f); // Add a small buffer distance
        var hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.20f, _wallLayerMask);
        var hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.20f, _wallLayerMask);
        _rb.linearVelocity = new Vector2(player.GetMoveInput().x * _moveSpeed, _rb.linearVelocity.y);
        CheckFirstMoveAndDirection();
        if (IsStuck()) return;
        if (player.GetMoveInput().x > 0 && !_isRight && _firstMove)
        {
            _animationConttroler.ChangeDirection(true);
            _isRight = true;
        }
        else if (player.GetMoveInput().x < 0 && _isRight && _firstMove)
        {
            _animationConttroler.ChangeDirection(false);
            _isRight = false;
        }
        else if (player.GetMoveInput().x == 0 && !_isStop && !_isFalling && _firstMove)
        {
            _animationConttroler.StopInMovement();
            SoundManager.Instance.StopSound();
            _isStop = true;
        }
        else if (player.GetMoveInput().x != 0 && _isStop && !_isFalling && _firstMove)
        {
            _animationConttroler.ResumeMovement();
            PlaySound(true, true, _moveSound);
            _isStop = false;
        }
        else
        {
            switch (_isFalling)
            {
                case false when hitLeft.collider is null && hitRight.collider is null:
                    _animationConttroler.ResumeMovement();
                    PlaySound(true, true, _fallingSound);
                    _isFalling = true;
                    _animationConttroler.FallWhileWalking();
                    break;
                case true when hitLeft.collider is not null || hitRight.collider is not null:
                    _isFalling = false;
                    _isStop = true;
                    _firstMove = false;
                    SoundManager.Instance.StopSound();
                    _animationConttroler.HitGroundWithMovement();
                    break;
            }
        }

        IsStuck();
    }

    public void SetIsRight(bool isRight)
    {
        _isRight = isRight;
    }

    public void SetHasJetPack(bool value)
    {
        _hasJetPack = value;
    }
}