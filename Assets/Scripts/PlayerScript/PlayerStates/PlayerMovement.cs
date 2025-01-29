using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/**
 * Player movement class that handles the movement of the player
 * and the transitions between the player states
 */
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Vector3 victoryWalkStart;
    [SerializeField] private LayerMask wallLayerMask;

    [FormerlySerializedAs("MoveSound")] [SerializeField]
    private AudioClip moveSound;

    [FormerlySerializedAs("FallingSound")] [SerializeField]
    private AudioClip fallingSound;

    [SerializeField] private AudioClip jumpSound;

    [FormerlySerializedAs("WinSound")] [SerializeField]
    private AudioClip winSound;

    [FormerlySerializedAs("StuckSound")] [SerializeField]
    private AudioClip stuckSound;

    [SerializeField] private AudioClip jetpackSound;
    [SerializeField] private float maxFuel; // Maximum fuel
    [SerializeField] private float airSpeed = -5f;
    [SerializeField] private float airTime = 0.7f;

    private PlayerAnimationConttroler _animationConttroler;
    private bool _canShoot;
    private Collider2D _collide;
    private InputSystem_Actions _controls;
    private PlayerState _currentState;
    private DeathState _deathState;
    private bool _hasJetPack;
    private bool _hasStarted;
    private bool _isRight;
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    private VictoryWalkState _victoryWalkState;
    public AirborneState AirborneState;
    public GroundedState GroundedState;
    public JetPackState JetPackState;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collide = GetComponent<Collider2D>();
        _controls = new InputSystem_Actions();
        _animationConttroler = GetComponent<PlayerAnimationConttroler>();
        GroundedState = new GroundedState(this);
        AirborneState = new AirborneState(this);
        _victoryWalkState = new VictoryWalkState(this);
        _deathState = new DeathState(this);
        if (CurrentLevelManagar.instance is not null)
            maxFuel = CurrentLevelManagar.instance.GetMaxFuel();
        JetPackState = new JetPackState(this);
    }

    /**
     * subscribe to the events of the level manager
     * in start because on awake the level manager
     * is not yet instantiated for some reason :(
     */
    private void Start()
    {
        CurrentLevelManagar.instance.LevelManager.OnVictoryWalkStart += StartVictoryWalk;
        CurrentLevelManagar.instance.FuelManager.OnJetPackChange += SetHasJetPack;
        CurrentLevelManagar.instance.PlayerManager.OnGunChange += SetCanShoot;
        OnVictoryWalkEnd += CurrentLevelManagar.instance.OnVictoryWalkEnd;
        _canShoot = CurrentLevelManagar.instance.GetCanShoot();
        _hasJetPack = CurrentLevelManagar.instance.GetCanFly();

        _rb.simulated = false;
    }

    private void Update()
    {
        if (_currentState == null) return;
        _currentState.HandleInput();
        _currentState.Update();
    }

    private void FixedUpdate()
    {
        _currentState?.FixedUpdate();
    }

    private void OnEnable()
    {
        _controls.Enable();
        _controls.Player.Move.performed += OnMove;
        _controls.Player.Jump.performed += OnJump;
        _controls.Player.Move.canceled += OnMove;
        _controls.Player.Quit.performed += context => Application.Quit();
    }

    private void OnDisable()
    {
        CurrentLevelManagar.instance.LevelManager.OnVictoryWalkStart -= StartVictoryWalk;
        CurrentLevelManagar.instance.FuelManager.OnJetPackChange -= SetHasJetPack;
        OnVictoryWalkEnd -= CurrentLevelManagar.instance.OnVictoryWalkEnd;
        _controls.Player.Move.performed -= OnMove;
        _controls.Player.Jump.performed -= OnJump;
        _controls.Player.Move.canceled -= OnMove;
        _controls.Disable();
    }

    public event Action OnVictoryWalkEnd;

    /**
     * method to transition to a new state
     * and exit the current state
     */
    public void TransitionToState(PlayerState state)
    {
        _currentState?.Exit();
        _currentState = state;
        _currentState.Enter();
    }

    /**
     * when player dies make the player fall and disable the collider,then move to death state
     */
    public void TriggerDeath()
    {
        _rb.gravityScale = 0.01f;
        _collide.enabled = false;
        TransitionToState(_deathState);
    }

    private void DestroyPlayer()
    {
        Destroy(gameObject);
    }

    private void StartVictoryWalk()
    {
        TransitionToState(_victoryWalkState);
    }

    public void TriggerVictoryWalkEnd()
    {
        OnVictoryWalkEnd?.Invoke();
    }

    /**
     * player movement method for most of the player states
     */
    public void MovePlayer()
    {
        _rb.linearVelocity = new Vector2(_moveInput.x * moveSpeed,
            _rb.linearVelocity.y);
    }

    /**
     * check for input to start the movement
     * of the player.
     */
    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        if (_moveInput.x != 0)
            CheckForStart();
    }

    public void PlaySound(bool shouldKeep, bool shouldLoop, AudioClip clip)
    {
        SoundManager.Instance.PlaySound(clip, transform, 1, shouldLoop, shouldKeep);
    }

    /**
     * check for input to start the movement
     * of the player.
     */
    private void OnJump(InputAction.CallbackContext context)
    {
        CheckForStart();
    }

    /**
     * check for the start of the game, if the game has not started then start the game and
     * set the player to be able to move,
     * then on second call to this function transition to the grounded state
     */
    private void CheckForStart()
    {
        if (_currentState == null && !_hasStarted)
        {
            _rb.simulated = true;
            _hasStarted = true;
            _animationConttroler.HitGroundWithoutMovement();
            return;
        }

        if (_hasStarted && _currentState == null) TransitionToState(AirborneState);
    }

// Start of getters and setters for the private fields for the player states.
    public LayerMask GetWallLayerMask()
    {
        return wallLayerMask;
    }
    public Rigidbody2D GetRigidbody()
    {
        return _rb;
    }

    public Vector2 GetMoveInput()
    {
        return _moveInput;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }


    public PlayerAnimationConttroler GetAnimationConttroler()
    {
        return _animationConttroler;
    }

    public Vector3 GetVictoryWalkStart()
    {
        return victoryWalkStart;
    }

    public InputSystem_Actions GetControls()
    {
        return _controls;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void SetTransform(Vector3 value)
    {
        transform.position = value;
    }

    public Collider2D GetCollider()
    {
        return _collide;
    }

    public AudioClip GetMoveSound()
    {
        return moveSound;
    }

    public AudioClip GetFallingSound()
    {
        return fallingSound;
    }

    public AudioClip GetJumpSound()
    {
        return jumpSound;
    }

    public AudioClip GetWinSound()
    {
        return winSound;
    }

    public void SetCanShoot(bool value)
    {
        _canShoot = value;
    }

    public bool GetCanShoot()
    {
        return _canShoot;
    }

    public AudioClip GetStuckSound()
    {
        return stuckSound;
    }

    public AudioClip GetJetpackSound()
    {
        return jetpackSound;
    }

    public bool GetHasJetPack()
    {
        return _hasJetPack;
    }

    public void SetIsRight(bool value)
    {
        _isRight = value;
    }

    public bool GetIsRight()
    {
        return _isRight;
    }

    private void SetHasJetPack(bool value)
    {
        _hasJetPack = value;
    }

    public float GetMaxFuel()
    {
        return maxFuel;
    }

    public float GetFallSpeed()
    {
        return airSpeed;
    }

    public float GetAirTime()
    {
        return airTime;
    }
}