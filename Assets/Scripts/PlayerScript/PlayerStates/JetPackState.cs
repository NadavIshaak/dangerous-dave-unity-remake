using UnityEngine;
using UnityEngine.UI;

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
    private bool _hasJetPack;
    private readonly Image _fuelBar; // The full fuel bar
    private readonly Image _blackBox; // The black box that indicates fuel depletion
    private readonly float _maxFuel;// Maximum fuel
    private static float _currentFuel;

    public JetPackState(PlayerMovement player) : base(player)
    {
        _controls = player.GetControls();
        _playerTransform = player.GetTransform();
        _rb = player.GetRigidbody();
        _animationConttroler = player.GetAnimationConttroler();
        _moveSpeed = player.GetMoveSpeed();
        _jetpackSound = player.GetJetpackSound();
        _fuelBar = player.GetFuelBar();
        _blackBox = player.GetBlackBox();
        _maxFuel = player.GetMaxFuel();
        _currentFuel = _maxFuel;
        UpdateFuelBar();
    }
    private void UpdateFuelBar()
    {
        float fuelPercentage = _currentFuel / _maxFuel;
        float blackBoxWidth = _fuelBar.rectTransform.rect.width * (1 - fuelPercentage);
        _blackBox.rectTransform.sizeDelta = new Vector2(blackBoxWidth, _blackBox.rectTransform.sizeDelta.y);
    }

    public override void Enter()
    {
        if (_currentFuel > 0)
        {
            PlaySound(true, true, _jetpackSound);
            _rb.gravityScale = 0; // Disable gravity
            _isFlying = true;
            _isRight = true;
        }
    }

    public override bool GetIsRight()
    {
        return _isRight;
    }

    public override void HandleInput()
    {
        if (_controls.Player.JetPack.IsPressed()==false||_currentFuel<=0)
        {
            player.TransitionToState(player.GroundedState); // Transition back to the previous state
        }
        else
        {
            _currentFuel -= Time.deltaTime * 10; // Decrease fuel
            UpdateFuelBar();
        }
    }

    public override void Update()
    {
        HandleInput();
        CheckInputAndAnimate();
    }
    

    public override void Exit()
    {
        if (_isFlying)
        {
            SoundManager.Instance.StopSound();
            _rb.gravityScale = 1; // Re-enable gravity
            _animationConttroler.HitGroundWithoutMovement();
        }
    }
    
    public static float GetCurrentFuel()
    {
        return _currentFuel;
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