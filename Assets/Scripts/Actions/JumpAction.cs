using UnityEngine;

public class JumpAction : Action
{
    public bool jumpGroundedOnly = true;
    public float jumpCooldown = 1f;
    public float jumpHeight = 1.5f;

    private float jumpVelocity;
    private float lastTimeJumped = -10f;

    protected override void Start()
    {
        base.Start();
        inputManager.A.action.performed += HandleJump;
    }

    private void HandleJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!isPermitted) return;

        if (jumpGroundedOnly && actionMediator.IsGrounded() ||
            (!jumpGroundedOnly && lastTimeJumped + jumpCooldown < Time.time))
        {
            Vector3 startPosition = transform.position;

            // Take input and estimate Direction
            Vector2 input2D = InputManager.Instance.leftJoystick.action.ReadValue<Vector2>();
            Transform relativeTransform = actionMediator.xRBodyTransformer.transform;
            Vector3 input = (relativeTransform.right * input2D.x + relativeTransform.forward * input2D.y).normalized;

            // Estimate velocity
            Vector3 estimatedVelocity = ((transform.position + (actionMediator.moveProvider.moveSpeed * Time.deltaTime * input)) 
                - startPosition) / Time.deltaTime;
            Debug.Log(actionMediator.moveProvider.moveSpeed * InputManager.Instance.leftJoystick.action.ReadValue<Vector2>());
            Vector3 horizontalVelocity = new Vector3(estimatedVelocity.x, 0f, estimatedVelocity.z);

            // Initialize jump
            actionMediator.SetPhysicalMotion(true);

            lastTimeJumped = Time.time;
            jumpVelocity = Mathf.Sqrt(1 * -Physics.gravity.y * jumpHeight);
            actionMediator.rb.linearVelocity =horizontalVelocity + Vector3.up * jumpVelocity;

            actionMediator.DisablePhysicalMotionOnLand();
        }
    }

    // Floating Jump? - More air time - reduced gravity
    private void FloatingJump()
    {
        Vector2 input2D = InputManager.Instance.leftJoystick.action.ReadValue<Vector2>();
        Transform reference = actionMediator.xRBodyTransformer.transform;
        Vector3 inputDir = (reference.right * input2D.x + reference.forward * input2D.y).normalized;

        
        float moveSpeed = actionMediator.moveProvider.moveSpeed;
        Vector3 estimatedVelocity = inputDir * moveSpeed;
        Vector3 horizontalVelocity = new Vector3(estimatedVelocity.x, 0f, estimatedVelocity.z);

        
        actionMediator.SetPhysicalMotion(true);
        lastTimeJumped = Time.time;

        float jumpVelocity = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        actionMediator.rb.linearVelocity = horizontalVelocity + Vector3.up * jumpVelocity;

        actionMediator.DisablePhysicalMotionOnLand();

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        inputManager.A.action.performed -= HandleJump;
    }
}
