using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Mover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Run-Jump Boost Settings")]
    [SerializeField] private float minRunSpeedForBoost = 1f;   // Horizontal speed from which boost starts to apply
    [SerializeField] private float maxExtraJumpForce = 4f;     // Maximum extra jump force when running at full speed

    [Header("Input Actions (Set in Inspector)")]
    [SerializeField] private InputAction moveRight = new InputAction(type: InputActionType.Button);
    [SerializeField] private InputAction moveLeft = new InputAction(type: InputActionType.Button);
    [SerializeField] private InputAction jump = new InputAction(type: InputActionType.Button);

    [Header("Raycast Settings")]
    [SerializeField] private Vector2 rayOffset = new Vector2(-1.9f, -3.5f);   // Start point of ground ray relative to player
    [SerializeField] private float groundRayDistance = 1.5f;               // Ground ray length
    [SerializeField] private LayerMask groundLayer;                        // Layer of the ground

    private Rigidbody2D rb;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        moveRight.Enable();
        moveLeft.Enable();
        jump.Enable();
    }

    private void OnDisable()
    {
        moveRight.Disable();
        moveLeft.Disable();
        jump.Disable();
    }

    private void Update()
    {
        // --- Ground Check with Raycast ---
        Vector2 origin = (Vector2)transform.position + rayOffset;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundRayDistance, groundLayer);
        isGrounded = hit.collider != null;

        // Draw the ray in the Scene view
        Debug.DrawRay(origin, Vector2.down * groundRayDistance, Color.red);

        // --- Horizontal Movement ---
        float horizontal = 0f;

        if (moveRight.IsPressed())
            horizontal = 1f;
        else if (moveLeft.IsPressed())
            horizontal = -1f;

        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        // --- Jump (only when grounded) ---
        if (jump.WasPerformedThisFrame() && isGrounded)
        {
            // Current horizontal speed magnitude
            float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);

            // Default jump force
            float finalJumpForce = jumpForce;

            // If the player is running fast enough, add extra jump force
            if (horizontalSpeed > minRunSpeedForBoost)
            {
                // Map horizontalSpeed from [minRunSpeedForBoost, moveSpeed] to [0, 1]
                float t = Mathf.InverseLerp(minRunSpeedForBoost, moveSpeed, horizontalSpeed);

                // Add extra jump force based on t (0 = no boost, 1 = full boost)
                float extraForce = t * maxExtraJumpForce;

                finalJumpForce += extraForce;
            }

            // Apply vertical velocity with boosted jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, finalJumpForce);
        }
    }
}
