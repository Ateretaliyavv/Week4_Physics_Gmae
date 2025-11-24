using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Mover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Run-Jump Boost Settings")]
    [SerializeField] private float minRunSpeedForBoost = 1f;
    [SerializeField] private float maxExtraJumpForce = 4f;

    [Header("Super Jump Power Up")]
    [SerializeField] private float superJumpForceMultiplier = 1.5f;
    [SerializeField] private float superJumpBoostMultiplier = 1.5f;

    [Header("Jump Power Down")]
    [SerializeField] private float weakJumpForceMultiplier = 0.6f;
    [SerializeField] private float weakJumpBoostMultiplier = 0.6f;

    [Header("Input Actions (Set in Inspector)")]
    [SerializeField] private InputAction moveRight = new InputAction(type: InputActionType.Button);
    [SerializeField] private InputAction moveLeft = new InputAction(type: InputActionType.Button);
    [SerializeField] private InputAction jump = new InputAction(type: InputActionType.Button);

    [Header("Raycast Settings")]
    [SerializeField] private Vector2 rayOffset = new Vector2(-1.9f, -3.5f);
    [SerializeField] private float groundRayDistance = 1.5f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    // Power states
    private bool superJumpActive = false;
    private bool weakJumpActive = false;
    private Coroutine superJumpCoroutine;

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
        // Ground check using raycast
        Vector2 origin = (Vector2)transform.position + rayOffset;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundRayDistance, groundLayer);
        isGrounded = hit.collider != null;

        Debug.DrawRay(origin, Vector2.down * groundRayDistance, Color.red);

        // Horizontal movement
        float horizontal = 0f;
        if (moveRight.IsPressed())
            horizontal = 1f;
        else if (moveLeft.IsPressed())
            horizontal = -1f;

        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        // Jump (only when grounded)
        if (jump.WasPerformedThisFrame() && isGrounded)
        {
            float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);

            // Base jump force
            float finalJumpForce = jumpForce;

            // Extra boost when running
            float extraForce = 0f;
            if (horizontalSpeed > minRunSpeedForBoost)
            {
                float t = Mathf.InverseLerp(minRunSpeedForBoost, moveSpeed, horizontalSpeed);
                extraForce = t * maxExtraJumpForce;
            }

            // Apply super jump power-up
            if (superJumpActive)
            {
                finalJumpForce *= superJumpForceMultiplier;
                extraForce *= superJumpBoostMultiplier;
            }

            // Apply weak jump debuff
            if (weakJumpActive)
            {
                finalJumpForce *= weakJumpForceMultiplier;
                extraForce *= weakJumpBoostMultiplier;
            }

            finalJumpForce += extraForce;

            // Apply jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, finalJumpForce);
        }
    }

    // Activate super jump
    public void ActivateSuperJump(float duration)
    {
        if (superJumpCoroutine != null)
        {
            StopCoroutine(superJumpCoroutine);
        }

        superJumpCoroutine = StartCoroutine(SuperJumpRoutine(duration));
    }

    private IEnumerator SuperJumpRoutine(float duration)
    {
        superJumpActive = true;
        yield return new WaitForSeconds(duration);
        superJumpActive = false;
        superJumpCoroutine = null;
    }

    // Activate weak jump
    public void ActivateWeakJump(float duration)
    {
        StartCoroutine(WeakJumpRoutine(duration));
    }

    private IEnumerator WeakJumpRoutine(float duration)
    {
        weakJumpActive = true;
        yield return new WaitForSeconds(duration);
        weakJumpActive = false;
    }
}
