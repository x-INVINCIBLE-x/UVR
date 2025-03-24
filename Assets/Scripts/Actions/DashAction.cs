using UnityEngine;

public class DashAction : Action
{
    public Rigidbody rb;

    public float dashForce;
    public float dashDuration = 0.5f;
    public Transform headTransform;
    private Vector3 direction;

    protected override void Start()
    {
        base.Start();

        inputManager.B.action.performed += Dash;

        inputManager.leftJoystick.action.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            direction = headTransform.right * input.x + headTransform.forward * input.y;
            direction.Normalize();
        };

        rb = actionMediator.rb;
    }

    private void Dash(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        actionMediator.SetPhysicalMotion(true);
        rb.AddForce(direction * dashForce, ForceMode.VelocityChange);
        actionMediator.DisablePhysicalMotion(dashDuration);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        inputManager.B.action.performed -= Dash;
    }
}