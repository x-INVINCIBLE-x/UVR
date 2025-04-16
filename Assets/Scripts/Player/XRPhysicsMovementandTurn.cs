using UnityEngine;
using UnityEngine.InputSystem;

public class XRPhysicsMovementAndTurn : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference moveAction; // Vector2 input (joystick)
    public InputActionReference turnAction; // Vector2 input (joystick)

    [Header("Settings")]
    public float moveSpeed = 2f;
    public float turnSpeed = 90f;
    public float deadZone = 0.2f;

    private Rigidbody rb;
    private Vector2 moveInput = Vector2.zero;
    private float turnInput = 0f;

    void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        turnAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        turnAction.action.Disable();
    }

    void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();

        Vector2 turnVec = turnAction.action.ReadValue<Vector2>();
        turnInput = Mathf.Abs(turnVec.x) > deadZone ? turnVec.x : 0f;
    }

    void FixedUpdate()
    {
        // Rotate using Rigidbody
        if (Mathf.Abs(turnInput) > 0f)
        {
            float angle = turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0f, angle, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }

        // Move using Rigidbody
        if (moveInput != Vector2.zero)
        {
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
            Vector3 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(targetPosition);
        }
    }
}
