using UnityEngine;
/**
 * This class is responsible for handling the jetpack state of the player.
 */
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

    /**
     * Update the fuel bar in the ui with the current fuel and max fuel
     */
    private void UpdateFuelBar()
    {
        CurrentLevelManagar.instance.UpdateFuelBar(_currentFuel, _maxFuel);
    }
    
    public override void Enter()
    {
        
            player.PlaySound(true, true, _jetpackSound);
            _animationConttroler.JetPack();
    }

    /**
     * Handle the input for the jetpack state, if the player presses the jetpack button or the fuel is empty,
     * transition back to the previous state, otherwise decrease the fuel.
     */
    public override void HandleInput()
    {
        if (_controls.Player.JetPack.WasPressedThisFrame() || _currentFuel <= 0)
        {
            CheckForEndOfJetpack();
        }
        else
        {
            DecreaseFuel();
        }
    }

    private void CheckForEndOfJetpack()
    {
        if (_currentFuel <= 0)
        {
            CurrentLevelManagar.instance.SetHasJetPack(false);
        }
        player.AirborneState.SetIsFalling(true);
        player.AirborneState.SetFromJetPack(true);
        player.TransitionToState(player.AirborneState); // Transition back to the previous state
    }

    /**
     * Decrease the fuel of the jetpack
     */
    private void DecreaseFuel()
    {
        _currentFuel -= Time.deltaTime * 5; // Decrease fuel
        CurrentLevelManagar.instance.SetCurrentJetPackFuel(_currentFuel);
        UpdateFuelBar();
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
    }

    public override void FixedUpdate()
    {
        // No fixed update during jetpack state
    }

    public static float GetCurrentFuel()
    {
        return _currentFuel;
    }


    /**
     * Check the input and animate the player
     */
    private void CheckInputAndAnimate()
    {
        var moveInput = player.GetMoveInput();
        _rb.linearVelocity = new Vector2(moveInput.x * _moveSpeed, moveInput.y * _moveSpeed);
        ChangeDirection(moveInput);
    }

    /**
     * Change the direction of the player
     */
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