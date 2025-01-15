
using UnityEngine;
using UnityEngine.InputSystem;
using System; 

public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions controls;
    private Vector2 moveInput;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Vector3 victoryWalkStart;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private AudioClip MoveSound;
    [SerializeField] private AudioClip FallingSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip WinSound;
    private Collider2D collide;
    private Rigidbody2D rb;
    private bool canShoot = false;
    private PlayerAnimationConttroler animationConttroler;
    public event Action OnVictoryWalkEnd;
    public PlayerState currentState;
    public GroundedState groundedState;
    public AirborneState airborneState;
    public VictoryWalkState victoryWalkState;
    public DeathState deathState;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collide = GetComponent<Collider2D>();
        animationConttroler = GetComponent<PlayerAnimationConttroler>();
        groundedState = new GroundedState(this);
        airborneState = new AirborneState(this);
        victoryWalkState = new VictoryWalkState(this);
        deathState = new DeathState(this);
        controls = new InputSystem_Actions();
    }
     private void Start()
    {
        GameManager.Instance.OnVictoryWalkStart += StartVictoryWalk;
        OnVictoryWalkEnd += GameManager.Instance.OnVictoryWalkEnd;
        OnVictoryWalkEnd += StageScript.Instance.OnEndWalk;
    }
    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnVictoryWalkStart -= StartVictoryWalk;
        OnVictoryWalkEnd -= GameManager.Instance.OnVictoryWalkEnd;
        OnVictoryWalkEnd -= StageScript.Instance.OnEndWalk;
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        Debug.Log("Disabled");
    }
    private void Update()
    {
        if (currentState == null)
        {
            return;
        }
        currentState.HandleInput();
        currentState.Update();
    }

    public void TransitionToState(PlayerState state)
    {
        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }
    public void TriggerDeath()
    {
        TransitionToState(deathState);
    }
     private void DestroyPlayer()
    {
        Destroy(gameObject);
    }

    private void StartVictoryWalk()
    {
        TransitionToState(victoryWalkState);
    }
     public void TriggerVictoryWalkEnd()
    {
        OnVictoryWalkEnd?.Invoke();
    }
    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (currentState == null&&moveInput.x!=0)
        {
            TransitionToState(groundedState);
        }
    }
    public LayerMask GetWallLayerMask() => wallLayerMask;
    public Rigidbody2D GetRigidbody() => rb;
    public Vector2 GetMoveInput() => moveInput;
    public float GetMoveSpeed() => moveSpeed;
    public float GetJumpForce() => jumpForce;
    public PlayerAnimationConttroler GetAnimationConttroler() => animationConttroler;
    public Vector3 GetVictoryWalkStart() => victoryWalkStart;
    public InputSystem_Actions GetControls() => controls;
    public Transform GetTransform() => transform;
    public void SetTransform(Vector3 value) => transform.position = value;
    public Collider2D GetCollider() => collide;
    public AudioClip GetMoveSound() => MoveSound;
    public AudioClip GetFallingSound() => FallingSound;
    public AudioClip GetJumpSound() => jumpSound;
    public AudioClip GetWinSound() => WinSound;
    public void SetCanShoot(bool value) => canShoot=value;
    public bool GetCanShoot() => canShoot;
}