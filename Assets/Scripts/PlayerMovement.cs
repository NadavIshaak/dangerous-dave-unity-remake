
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions controls;
    private Vector2 moveInput;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Vector3 victoryWalkStart;

    private Rigidbody2D rb;
    private PlayerAnimationConttroler animationConttroler;
    public event Action OnVictoryWalkEnd;
      private bool isVictoryWalking = false;

    // Tracks if we pressed horizontal input in the current frame
    private bool didMove = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animationConttroler = GetComponent<PlayerAnimationConttroler>();
        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        controls.Player.Jump.performed += OnJump;
         GameManager.Instance.OnVictoryWalkStart += StartVictoryWalk;
        OnVictoryWalkEnd += GameManager.Instance.OnVictoryWalkEnd;
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Player.Jump.performed -= OnJump;
        controls.Disable();
         GameManager.Instance.OnVictoryWalkStart -= StartVictoryWalk;
        OnVictoryWalkEnd -= GameManager.Instance.OnVictoryWalkEnd;
    }

    private void Update()
    {
        if(!isVictoryWalking)
        {
        // Horizontal movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Call animation update if we moved or jumpe
        // (Dangerous Dave anim typically flips between left/right frames or idle frames)
            animationConttroler.Move(moveInput.x, rb.linearVelocity.y);
        }
        else{
            rb.linearVelocity = new Vector2(moveSpeed,0);
            animationConttroler.Move(moveSpeed,0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 0.5f, wallLayerMask);
            if (hit.collider != null)
            {
                isVictoryWalking = false;
                OnVictoryWalkEnd?.Invoke();
                Destroy(gameObject);
            }
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // Simple jump if on the ground (approx: linear velocity y == 0)
        if (Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animationConttroler.Jump();
        }
    }
     private void StartVictoryWalk()
    {
        // Teleport the player to the victory walk area
        transform.position = victoryWalkStart;
        isVictoryWalking = true;
    }
}