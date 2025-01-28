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
        
            player.PlaySound(true, true, _jetpackSound);
            _animationConttroler.JetPack();
      
    }


    public override void HandleInput()
    {
        if (_controls.Player.JetPack.WasPressedThisFrame() || _currentFuel <= 0)
        {
            if (_currentFuel <= 0)
            {
                CurrentLevelManagar.instance.SetHasJetPack(false);
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
            SoundManager.Instance.StopSound();
            _animationConttroler.ResumeMovement();
            _animationConttroler.HitGroundWithoutMovement();
        Debug.Log("Exit JetPackState");
    }

    public override void FixedUpdate()
    {
        // No fixed update during jetpack state
    }

    public static float GetCurrentFuel()
    {
        return _currentFuel;
    }


    private void CheckInputAndAnimate()
    {
        var moveInput = player.GetMoveInput();
        _rb.linearVelocity = new Vector2(moveInput.x * _moveSpeed, moveInput.y * _moveSpeed);
        ChangeDirection(moveInput);
    }

    private void ChangeDirection(Vector2 moveInput)
    {
        var isRight = player.GetIsRight();
        switch (moveInput.x)
        {
            case > 0:
                if (isRight) return;
                _animationConttroler.ChangeDirection(true);
                player.SetIsRight(true);
                break;
            case < 0:
                if (!isRight) return;
                _animationConttroler.ChangeDirection(false);
                player.SetIsRight(false);
                break;
        }
    }
}