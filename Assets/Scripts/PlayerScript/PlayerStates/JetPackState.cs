using UnityEngine;

public class JetPackState : PlayerState
{
    private static float _currentFuel;
    private readonly PlayerAnimationConttroler _animationConttroler;
    private readonly InputSystem_Actions _controls;
    private readonly AudioClip _jetpackSound;
    private readonly float _maxFuel; // Maximum fuel
    private readonly float _moveSpeed;
    private readonly Rigidbody2D _rb;
    private bool _isFlying;
    

    public JetPackState(PlayerMovement player) : base(player)
    {
        _controls = player.GetControls();
        _rb = player.GetRigidbody();
        _animationConttroler = player.GetAnimationConttroler();
        _moveSpeed = player.GetMoveSpeed();
        _jetpackSound = player.GetJetpackSound();
        _maxFuel = 100f;
        _currentFuel = player.GetMaxFuel();
        UpdateFuelBar();
    }

    private void UpdateFuelBar()
    {
        CurrentLevelManagar.instance.UpdateFuelBar(_currentFuel, _maxFuel);
    }


    public override void Enter()
    {
        if (_currentFuel > 0)
        {
            player.PlaySound(true, true, _jetpackSound);
            _rb.gravityScale = 0; // Disable gravity
            _isFlying = true;
        }
    }
    

    public override void HandleInput()
    {
        if (_controls.Player.JetPack.WasPressedThisFrame() || _currentFuel <= 0)
        {
            if (_currentFuel <= 0)
            {
                CurrentLevelManagar.instance.SetHasJetPack(false);
                player.SetHasJetPack(false);
            }
            player.AirborneState.SetIsFalling(true);
            player.TransitionToState(player.AirborneState); // Transition back to the previous state
        }
        else
        {
            _currentFuel -= Time.deltaTime * 5; // Decrease fuel
            CurrentLevelManagar.instance.SetCurrentJetPackFuel(_currentFuel);
            UpdateFuelBar();
        }
    }

    public override void Update()
    {
        CheckInputAndAnimate();
    }


    public override void Exit()
    {
        if (_isFlying)
        {
            SoundManager.Instance.StopSound();
            _rb.gravityScale = 0.95f; // Re-enable gravity
            _animationConttroler.HitGroundWithoutMovement();
        }

        Debug.Log("Exit JetPackState");
      
    }

    public static float GetCurrentFuel()
    {
        return _currentFuel;
    }



    private void CheckInputAndAnimate()
    {
        var moveInput = player.GetMoveInput();
        _rb.linearVelocity = new Vector2(moveInput.x * _moveSpeed, moveInput.y * _moveSpeed);

        if (_isFlying) _animationConttroler.JetPack();
        switch (moveInput.x)
        {
            case > 0:
                _animationConttroler.ChangeDirection(true);
                player.SetIsRight(true);
                break;
            case < 0:
                _animationConttroler.ChangeDirection(false);
                player.SetIsRight(false);
                break;
        }
    }
}