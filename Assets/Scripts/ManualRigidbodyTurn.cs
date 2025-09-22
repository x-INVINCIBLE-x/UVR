using UnityEngine;
using UnityEngine.InputSystem;

public class ManualRigidbodyTurn : MonoBehaviour
{
    public InputActionReference turnInput;   // Left/Right stick
    public float torqueStrength = 50f;       // Strength of turning force
    public float maxAngularVelocity = 10f;   // Prevents excessive spin
    public float angularDragWhenIdle = 20f;  // High drag when no input
    public float angularDragWhenTurning = 0.05f; // Low drag when turning

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;

        if (turnInput != null)
            turnInput.action.Enable();
    }

    private void OnEnable()
    {
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        // Fully freeze rotations at start (neglect all physics rotation)
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        if (rb == null || turnInput == null) return;

        float turnInputValue = turnInput.action.ReadValue<Vector2>().x;
        turnInputValue = Mathf.Clamp(turnInputValue, -1f, 1f);

        if (Mathf.Abs(turnInputValue) > 0.01f)
        {
            // Allow Y rotation ONLY while turning
            rb.constraints &= ~RigidbodyConstraints.FreezeRotationY;

            float torque = turnInputValue * torqueStrength;
            rb.AddTorque(Vector3.up * torque, ForceMode.Impulse);

            rb.angularDamping = angularDragWhenTurning;
        }
        else
        {
            // Lock rotation again & stop movement
            rb.constraints |= RigidbodyConstraints.FreezeRotationY;
            rb.angularVelocity = Vector3.zero;
            rb.angularDamping = angularDragWhenIdle;
        }
    }

    private void OnDisable()
    {
        rb.interpolation = RigidbodyInterpolation.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
