using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class DashAction : Action
{
    public Rigidbody rb;

    public float dashForce;
    public float dashDuration = 0.5f;
    public Transform headTransform;
    private Vector3 direction;

    public bool canSwingDash = true;
    private float defaultDashForce;
    private Coroutine dashCoroutine;
    private Vector2 input;

    protected override void Start()
    {
        base.Start();

        defaultDashForce = dashForce;
        //actionMediator.OnTimeModified += HandleTimeModification;

        inputManager.B.action.performed += Dash;

        inputManager.leftJoystick.action.performed += ctx =>
        {
            input = ctx.ReadValue<Vector2>();
        };

        rb = actionMediator.rb;
    }

    private void Dash(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!CanUseAbility()) return;
        if (!canSwingDash && actionMediator.grabStatus.IsSwinging()) return;
        if (actionMediator.grabStatus.IsClimbing()) return;

        if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);

        lastTimeSkillUsed = Time.time;
        dashCoroutine = StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        //isActive = true;
        actionMediator.immuneInterpolation = true;
        actionMediator.SetPhysicalMotion(true, true);
        float timer = 0f;

        direction = headTransform.right * input.x + headTransform.forward * input.y;
        direction.y = 0f;
        direction.Normalize();

        Vector3 dashVelocity = direction * dashForce;

        while (timer < dashDuration)
        {
            rb.linearVelocity = dashVelocity * (1/Time.timeScale);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        //isActive = false;
        actionMediator.immuneInterpolation = false;
        rb.linearVelocity = Vector3.zero;

        if (actionMediator.IsGrounded())
            actionMediator.DisablePhysicalMotion(0f, true);
        else
            actionMediator.DisablePhysicalMotionOnLand(true);
    }

    //private void HandleTimeModification(float modifier)
    //{
    //    if (isActive)
    //        actionMediator.rb.interpolation = RigidbodyInterpolation.Interpolate;
    //}

    protected override void OnDestroy()
    {
        base.OnDestroy();

        inputManager.B.action.performed -= Dash;
    }
}