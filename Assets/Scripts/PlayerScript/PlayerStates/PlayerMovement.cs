using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Vector3 victoryWalkStart;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private AudioClip MoveSound;
    [SerializeField] private AudioClip FallingSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip WinSound;
    [SerializeField] private AudioClip StuckSound;
    [SerializeField] private AudioClip jetpackSound;
    [SerializeField] private float maxFuel = 100f; // Maximum fuel
    [SerializeField] private float airSpeed = -5f;
    [SerializeField] private float airTime = 0.7f;

    private PlayerAnimationConttroler _animationConttroler;
    private bool _canShoot;
    private Collider2D _collide;
    private InputSystem_Actions _controls;
    private bool _hasJetPack;
    private bool _hasStarted;
    private bool _isRight;
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    public AirborneState AirborneState;
    public PlayerState CurrentState;
    public DeathState DeathState;
    public GroundedState GroundedState;
    public JetPackState JetPackState;
    public VictoryWalkState VictoryWalkState;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collide = GetComponent<Collider2D>();
        _controls = new InputSystem_Actions();
        _animationConttroler = GetComponent<PlayerAnimationConttroler>();
        GroundedState = new GroundedState(this);
        AirborneState = new AirborneState(this);
        VictoryWalkState = new VictoryWalkState(this);
        DeathState = new DeathState(this);
        JetPackState = new JetPackState(this);
    }

    private void Start()
    {
        CurrentLevelManagar.instance.OnVictoryWalkStart += StartVictoryWalk;
        OnVictoryWalkEnd += CurrentLevelManagar.instance.OnVictoryWalkEnd;
        OnVictoryWalkEnd += StageScript.Instance.OnEndWalk;
        _canShoot = CurrentLevelManagar.instance.GetCanShoot();
        _rb.simulated = false;
        maxFuel=CurrentLevelManagar.instance.GetMaxFuel();
    }

    private void Update()
    {
        if (CurrentState == null) return;
        CurrentState.HandleInput();
        CurrentState.Update();
    }

    private void OnEnable()
    {
        _controls.Enable();
        _controls.Player.Move.performed += OnMove;
        _controls.Player.Jump.performed += OnJump;
        _controls.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        CurrentLevelManagar.instance.OnVictoryWalkStart -= StartVictoryWalk;
        OnVictoryWalkEnd -= CurrentLevelManagar.instance.OnVictoryWalkEnd;
        OnVictoryWalkEnd -= StageScript.Instance.OnEndWalk;
        _controls.Player.Move.performed -= OnMove;
        _controls.Player.Jump.performed -= OnJump;
        _controls.Player.Move.canceled -= OnMove;
        _controls.Disable();
    }

    public event Action OnVictoryWalkEnd;

    public void TransitionToState(PlayerState state)
    {
        CurrentState?.Exit();
        CurrentState = state;
        CurrentState.Enter();
    }

    public void TriggerDeath()
    {
        _rb.gravityScale = 0.1f;
        _collide.enabled = false;
        TransitionToState(DeathState);
    }

    private void DestroyPlayer()
    {
        Destroy(gameObject);
    }

    private void StartVictoryWalk()
    {
        TransitionToState(VictoryWalkState);
    }

    public void TriggerVictoryWalkEnd()
    {
        OnVictoryWalkEnd?.Invoke();
    }

    public void MovePlayer()
    {
        _rb.linearVelocity = new Vector2(_moveInput.x * moveSpeed,
            _rb.linearVelocity.y);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        if(_moveInput.x!=0)
           CheckForStart();
    }
    public void PlaySound(bool shouldKeep, bool shouldLoop, AudioClip clip)
    {
        SoundManager.Instance.PlaySound(clip, transform, 1, shouldLoop, shouldKeep);
    } 

    private void OnJump(InputAction.CallbackContext context)
    {
        CheckForStart();
    }

    private void CheckForStart()
    {
        if (CurrentState == null && !_hasStarted)
        {
            _rb.simulated = true;
            _hasStarted = true;
            _animationConttroler.HitGroundWithoutMovement();
            return;
        }

        if (_hasStarted && CurrentState == null) TransitionToState(GroundedState);
    }

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
        return MoveSound;
    }

    public AudioClip GetFallingSound()
    {
        return FallingSound;
    }

    public AudioClip GetJumpSound()
    {
        return jumpSound;
    }

    public AudioClip GetWinSound()
    {
        return WinSound;
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
        return StuckSound;
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

    public void SetHasJetPack(bool value)
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