using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    public float gravity = -9.81f;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundMask;
    public Transform groundCheck;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool gravityEnabled = true;

    void Start()
    {
        controller = GetComponentInChildren<CharacterController>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (gravityEnabled)
        {
            velocity.y += gravity * Time.deltaTime;
            if (controller.enabled)
                controller.Move(velocity * Time.deltaTime);
        }
    }

    public void SetYVelocity(float velocity) => this.velocity.y = velocity;

    public void EnableGravity()
    {
        gravityEnabled = true;
    }

    public void DisableGravity()
    {
        gravityEnabled = false;
        velocity.y = 0f; 
    }

    public bool IsGrounded() => isGrounded;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckDistance);
    }
}
