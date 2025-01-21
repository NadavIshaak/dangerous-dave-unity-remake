using UnityEngine;

public class GroundedState : PlayerState
{
    private readonly PlayerAnimationConttroler _animationConttroler;
    private readonly Collider2D _collider;
    private readonly InputSystem_Actions _controls;
    private readonly float _jumpForce;
    private readonly AudioClip _moveSound;
    private readonly Transform _playerTransform;
    private readonly Rigidbody2D _rb;
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
        _jumpForce = player.GetJumpForce();
        _collider = player.GetCollider();
        _rb = player.GetRigidbody();
        _wallLayerMask = player.GetWallLayerMask();
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
        if (!_controls.Player.Jump.triggered) return;
        JumpTransition();
    }
    public override void Update() { CheckInputAndAnimate(); }

    public override void Exit() { _animationConttroler.ResumeMovement();}
    
    private void JumpTransition()
    {
        player.AirborneState.SetIsFalling(false);
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
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
        if ((hitLeft.collider is not null || hitTopLeft.collider is not null) && player.GetMoveInput().x < 0
            ||((hitRight.collider is not null || hitTopRight.collider is not null) && player.GetMoveInput().x > 0))
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
    private void CheckForNoFuel()
    {
        _hasJetPack = player.GetHasJetPack();
        if (_controls.Player.JetPack.WasPressedThisFrame() && _hasJetPack&&!_justTransitioned)
            if (JetPackState.GetCurrentFuel() > 0)
                player.TransitionToState(player.JetPackState);
        _justTransitioned = false;
    }
    private void CheckForFall()
    {
        var bounds = _collider.bounds;
        var bottomLeft = new Vector2(bounds.min.x, bounds.min.y + 0.1f); // Add a small buffer distance
        var bottomRight = new Vector2(bounds.max.x, bounds.min.y + 0.1f); // Add a small buffer distance
        var hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.20f, _wallLayerMask);
        var hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.20f, _wallLayerMask);
        if (hitLeft.collider is null && hitRight.collider is null)
        {
            FallTransition();
        }
    }
    private void CheckInputAndAnimate()
    {
        CheckForFall();
        CheckForNoFuel();
        player.MovePlayer();
        if (IsStuck()) return;
        if (player.GetMoveInput().x != 0) ChangeDirectionOfMovement();
        
        if (player.GetMoveInput().x == 0 && !_isStop && _firstMove)
            StopMovement();
        else if (player.GetMoveInput().x != 0 && _isStop && _firstMove)
            ContinueMovementAfterStop();
        
    }
    private void ChangeDirectionOfMovement()
    {
        if (!_firstMove)
        {
            _animationConttroler.ResumeMovement();
            _animationConttroler.Move();
            player.PlaySound(true, true, _moveSound);
        }

        _firstMove = true;
        _animationConttroler.ChangeDirection(player.GetMoveInput().x > 0);
        player.SetIsRight(player.GetMoveInput().x > 0);
    }

    private void StopMovement()
    {
        _animationConttroler.StopInMovement();
        SoundManager.Instance.StopSound();
        _isStop = true;
    }

    private void ContinueMovementAfterStop()
    {
        _animationConttroler.ResumeMovement();
        player.PlaySound(true, true, _moveSound);
        _isStop = false;
    }
}