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

    private void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Space))
        //    HandleJump();
    }

    private void HandleJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!isPermitted) return;

        if (jumpGroundedOnly && actionMediator.IsGrounded() ||
            (!jumpGroundedOnly && lastTimeJumped + jumpCooldown > Time.deltaTime))
        {
            actionMediator.SetPhysicalMotion(true);

            lastTimeJumped = Time.time;
            jumpVelocity = Mathf.Sqrt(1 * -Physics.gravity.y * jumpHeight);
            actionMediator.rb.linearVelocity = Vector3.up * jumpVelocity;

            actionMediator.DisablePhysicalMotionOnLand();
        }
    }

    //private void HandleJump()
    //{
    //    if (jumpGroundedOnly && actionMediator.IsGrounded() ||
    //        (!jumpGroundedOnly && lastTimeJumped + jumpCooldown > Time.deltaTime))
    //    {
    //        actionMediator.SetPhysicalMotion(true);

    //        lastTimeJumped = Time.time;
    //        jumpVelocity = Mathf.Sqrt(1 * -Physics.gravity.y * jumpHeight);
    //        actionMediator.rb.linearVelocity = Vector3.up * jumpVelocity;

    //        actionMediator.DisablePhysicalMotionOnLand();
    //    }
    //}

    protected override void OnDestroy()
    {
        base.OnDestroy();

        inputManager.A.action.performed -= HandleJump;
    }
}
