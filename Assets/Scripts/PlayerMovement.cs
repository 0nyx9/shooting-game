using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
   public float moveSpeed = 5f;
   public float jumpForce = 5f;

   public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Rigidbody rb;
    private Vector3 moveInput;
    private bool isGrounded;
    // public PlayerInput playerInput;
    public UnityEngine.InputSystem.PlayerInput playerInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // playerInput = GetComponent<PlayerInput>();
        playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
    }

    void Update()
    {
        CheckGround();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void OnMovement(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void MovePlayer()
    {
        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        direction.Normalize();
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);
    }
}
