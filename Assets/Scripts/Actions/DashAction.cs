using UnityEngine;

public class DashAction : Action
{
    public Rigidbody rb;

    public float dashForce;
    public float dashDuration = 0.5f;
    public Transform headTransform;
    private Vector3 direction;

    public bool canSwingDash = true;
    private float defaultDashForce;

    protected override void Start()
    {
        base.Start();

        defaultDashForce = dashForce;
        actionMediator.OnTimeModified += HandleTimeModification;

        inputManager.B.action.performed += Dash;

        inputManager.leftJoystick.action.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            direction = headTransform.right * input.x + headTransform.forward * input.y;
            direction.y = 0f;
            direction.Normalize();
        };

        rb = actionMediator.rb;
    }

    private void Dash(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!canSwingDash && actionMediator.grabStatus.IsSwinging()) return;

        if (actionMediator.grabStatus.IsClimbing()) return;

        actionMediator.SetPhysicalMotion(true);
        rb.AddForce(direction * dashForce, ForceMode.VelocityChange);
        // - Changes-
        if (actionMediator.IsGrounded())
            actionMediator.DisablePhysicalMotion(dashDuration);
        else
            actionMediator.DisablePhysicalMotionOnLand();
    }

    private void HandleTimeModification(float modifier)
    {
        dashForce = modifier == 1 ? defaultDashForce : defaultDashForce * (1 / Time.timeScale);    
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        inputManager.B.action.performed -= Dash;
    }
}