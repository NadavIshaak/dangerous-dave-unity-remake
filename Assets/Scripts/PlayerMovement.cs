// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerMovement : MonoBehaviour
// {
//     private InputSystem_Actions controls;
//     private Vector2 moveInput;
//     [SerializeField] private float moveSpeed = 5f;
//     [SerializeField] private float jumpForce = 8f;
//     private PlayerAnimationConttroler animationConttroler;
//     private Rigidbody2D rb;
//     private bool didMove=false;


//     private void Awake()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         controls = new InputSystem_Actions();
//         animationConttroler = GetComponent<PlayerAnimationConttroler>();
//     }

//     private void OnEnable()
//     {
//         controls.Enable();
//         controls.Player.Move.performed += OnMove;
//         controls.Player.Move.canceled += OnMove;
//         controls.Player.Jump.performed += OnJump;
//     }

//     private void OnDisable()
//     {
//         controls.Player.Move.performed -= OnMove;
//         controls.Player.Move.canceled -= OnMove;
//         controls.Player.Jump.performed -= OnJump;
//         controls.Disable();
//     }

//     private void Update()
//     {
//         // Horizontal movement
//         rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
//         if(didMove==true)
//         animationConttroler.Move(moveInput.x,rb.linearVelocity.y);
//     }

//     private void OnMove(InputAction.CallbackContext context)
//     {
//         moveInput = context.ReadValue<Vector2>();
//         if (moveInput.x!=0)
//         {
//             didMove = true;
//         }
        
//     }

//     private void OnJump(InputAction.CallbackContext context)
//     {
//         if (rb.linearVelocity.y==0)
//         {
//             rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
//             animationConttroler.Jump();
//         }
//     }



   
// }
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions controls;
    private Vector2 moveInput;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;

    private Rigidbody2D rb;
    private PlayerAnimationConttroler animationConttroler;

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
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Player.Jump.performed -= OnJump;
        controls.Disable();
    }

    private void Update()
    {
        // Horizontal movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Call animation update if we moved or jumpe
        // (Dangerous Dave anim typically flips between left/right frames or idle frames)
        if (didMove)
        {
            animationConttroler.Move(moveInput.x, rb.linearVelocity.y);
        }
        else
        {
            // Even if we're standing still, send the current speed to handle idle/landing
            animationConttroler.Move(0, rb.linearVelocity.y);
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        didMove = (moveInput.x != 0);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // Simple jump if on the ground (approx: linear velocity y == 0)
        if (Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animationConttroler.Jump(moveInput.x);
        }
    }
}