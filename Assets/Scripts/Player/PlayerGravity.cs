using System.Collections;
using UnityEngine;

public enum GroundedState
{
    Grounded,
    Falling
}

public class PlayerGravity : MonoBehaviour
{
    public float gravity = -9.81f;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundMask;
    public Transform groundCheck;
    public CharacterController controller;

    [Header("Fall Detection")]
    public float fallThreshold = -15f;

    private Vector3 velocity;
    [SerializeField] private bool isGrounded;
    private bool gravityEnabled = true;

    // --- New: State tracking ---
    public GroundedState CurrentState { get; private set; } = GroundedState.Grounded;
    private GroundedState previousState;

    void Update()
    {
        // --- Ground Check ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // --- Gravity ---
        if (gravityEnabled)
        {
            velocity.y += gravity * Time.deltaTime;
            if (controller.enabled)
                controller.Move(velocity * Time.deltaTime);
        }

        // --- Update State (efficient) ---
        UpdateGroundedState();

        // --- Fall Check ---
        if (CurrentState == GroundedState.Falling && velocity.y < fallThreshold)
        {
            OnFallDetected();
        }
    }

    private void UpdateGroundedState()
    {
        // Decide state from grounded flag
        CurrentState = isGrounded ? GroundedState.Grounded : GroundedState.Falling;

        // Only react if state actually changed
        if (CurrentState != previousState)
        {
            previousState = CurrentState;
            OnStateChanged(CurrentState);
        }
    }

    private void OnStateChanged(GroundedState newState)
    {
        // Callback hook (optional)
        Debug.Log($"Grounded state changed: {newState}");
    }

    public void SetYVelocity(float velocity) => this.velocity.y = velocity;

    public void EnableGravity() => gravityEnabled = true;

    public void DisableGravity()
    {
        gravityEnabled = false;
        velocity.y = 0f;
    }

    public bool IsGrounded() => isGrounded;

    public float GetYVelocity() => velocity.y;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckDistance);
    }

    private void OnFallDetected()
    {
        Transform playerBody = PlayerManager.instance.PlayerOrigin.transform;
        Debug.Log($"Player is falling fast! Y velocity: {velocity.y}, Position: {playerBody.position}");
    }
}
