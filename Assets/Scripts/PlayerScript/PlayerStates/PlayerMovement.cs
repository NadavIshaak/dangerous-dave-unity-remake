
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions _controls;
    private Vector2 _moveInput;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Vector3 victoryWalkStart;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private AudioClip MoveSound;
    [SerializeField] private AudioClip FallingSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip WinSound;
    [SerializeField] private AudioClip StuckSound;
    [SerializeField] private AudioClip jetpackSound;
    [SerializeField] private float maxFuel = 100f; // Maximum fuel
    private Collider2D _collide;
    private Rigidbody2D _rb;
    private bool _canShoot = false;
    private PlayerAnimationConttroler _animationConttroler;
    public event Action OnVictoryWalkEnd;
    public PlayerState CurrentState;
    public GroundedState GroundedState;
    public AirborneState AirborneState;
    public VictoryWalkState VictoryWalkState;
    public DeathState DeathState;
    public JetPackState JetPackState;
    private bool _hasJetPack;

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
        GameManager.instance.OnVictoryWalkStart += StartVictoryWalk;
        OnVictoryWalkEnd += GameManager.instance.OnVictoryWalkEnd;
        OnVictoryWalkEnd += StageScript.Instance.OnEndWalk;
        _canShoot= GameManager.instance.GetCanShoot();
    }
    private void OnEnable()
    {
        _controls.Enable();
        _controls.Player.Move.performed += OnMove;
        _controls.Player.Move.canceled += OnMove;
    }
    private void OnDisable()
    {
        GameManager.instance.OnVictoryWalkStart -= StartVictoryWalk;
        OnVictoryWalkEnd -= GameManager.instance.OnVictoryWalkEnd;
        OnVictoryWalkEnd -= StageScript.Instance.OnEndWalk;
        _controls.Player.Move.performed -= OnMove;
        _controls.Player.Move.canceled -= OnMove;
        _controls.Disable();
    }
    private void Update()
    {
        if (CurrentState == null)
        {
            return;
        }
        CurrentState.HandleInput();
        CurrentState.Update();
    }

    public void TransitionToState(PlayerState state)
    {
        CurrentState?.Exit();
        CurrentState = state;
        CurrentState.Enter();
    }
    public void TriggerDeath()
    {
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
    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        if (CurrentState == null&&_moveInput.x!=0)
        {
            TransitionToState(GroundedState);
        }
    }
    public LayerMask GetWallLayerMask() => wallLayerMask;
    public Rigidbody2D GetRigidbody() => _rb;
    public Vector2 GetMoveInput() => _moveInput;
    public float GetMoveSpeed() => moveSpeed;
    public float GetJumpForce() => jumpForce;
    public PlayerAnimationConttroler GetAnimationConttroler() => _animationConttroler;
    public Vector3 GetVictoryWalkStart() => victoryWalkStart;
    public InputSystem_Actions GetControls() => _controls;
    public Transform GetTransform() => transform;
    public void SetTransform(Vector3 value) => transform.position = value;
    public Collider2D GetCollider() => _collide;
    public AudioClip GetMoveSound() => MoveSound;
    public AudioClip GetFallingSound() => FallingSound;
    public AudioClip GetJumpSound() => jumpSound;
    public AudioClip GetWinSound() => WinSound;
    public void SetCanShoot(bool value) => _canShoot=value;
    public bool GetCanShoot() => _canShoot;
    public AudioClip GetStuckSound() => StuckSound;
    public  AudioClip GetJetpackSound() => jetpackSound;
    public bool GetHasJetPack() => _hasJetPack;

    public void SetHasJetPack(bool value)
    {
        _hasJetPack = value;
        GroundedState.SetHasJetPack(value);
        AirborneState.SetHasJetPack(value);
    }
    public float GetMaxFuel() => maxFuel;
    
}