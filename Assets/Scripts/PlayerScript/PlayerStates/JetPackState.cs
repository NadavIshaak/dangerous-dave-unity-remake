using UnityEngine;

public class JetPackState : PlayerState
{
    private readonly InputSystem_Actions _controls;
    private readonly Transform _playerTransform;
    private readonly Rigidbody2D _rb;
    private readonly PlayerAnimationConttroler _animationConttroler;
    private readonly float _moveSpeed;
    private readonly AudioClip _jetpackSound;
    private bool _isFlying;
    private bool _isRight;

    public JetPackState(PlayerMovement player) : base(player)
    {
        _controls = player.GetControls();
        _playerTransform = player.GetTransform();
        _rb = player.GetRigidbody();
        _animationConttroler = player.GetAnimationConttroler();
        _moveSpeed = player.GetMoveSpeed();
        _jetpackSound = player.GetJetpackSound();
    }

    public override void Enter()
    {
        _isFlying = true;
        _rb.gravityScale = 0; // Disable gravity
        _isRight = true;
        PlaySound(true, true, _jetpackSound);
    }

    public override bool GetIsRight()
    {
        return _isRight;
    }

    public override void HandleInput()
    {
        if (_controls.Player.JetPack.IsPressed()==false)
        {
            _isFlying = false;
            player.TransitionToState(player.GroundedState); // Transition back to the previous state
        }
    }

    public override void Update()
    {
        HandleInput();
        CheckInputAndAnimate();
    }

    public override void Exit()
    {
        SoundManager.Instance.stopSound();
        _rb.gravityScale = 1; // Re-enable gravity
    }

    private void PlaySound(bool shouldKeep, bool shouldLoop, AudioClip clip)
    {
        SoundManager.Instance.PlaySound(clip, _playerTransform, 1, shouldLoop, shouldKeep);
    }

    private void CheckInputAndAnimate()
    {
        var moveInput = player.GetMoveInput();
        _rb.linearVelocity = new Vector2(moveInput.x * _moveSpeed, moveInput.y * _moveSpeed);

        if (_isFlying)
        {
            _animationConttroler.JetPack();
        }
        switch (moveInput.x)
        {
            case > 0:
                _animationConttroler.ChangeDirection(true);
                _isRight = true;
                break;
            case < 0:
                _animationConttroler.ChangeDirection(false);
                _isRight = false;
                break;
        }
    }
}