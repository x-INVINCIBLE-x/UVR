using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class DashAction : Action
{
    public Rigidbody rb;
    public CapsuleCollider bodyCollider;
    public LayerMask obstacleLayerMask;

    public float dashForce;
    public float dashDuration = 0.5f;
    public float safeDashDistance = 2f;
    public Transform headTransform;
    private Vector3 direction;

    public bool canSwingDash = true;
    private float defaultDashForce;
    private Coroutine dashCoroutine;
    private Vector2 input;

    private Vector3 heightOffset = new Vector3(0f, 0.5f, 0f);

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
        bodyCollider = actionMediator.bodyCollider.GetComponent<CapsuleCollider>();
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
        //actionMediator.immuneInterpolation = true;
        //actionMediator.SetPhysicalMotion(true, true);

        float timer = 0f;

        // Compute dash direction once
        direction = headTransform.right * input.x + headTransform.forward * input.y;
        direction.y = 0f;
        direction.Normalize();

        Vector3 dashVelocity = direction * dashForce;

        while (timer < dashDuration)
        {
            // Distance to move this frame
            float step = dashVelocity.magnitude * Time.unscaledDeltaTime;
            Vector3 move = direction * step;

            // Check if obstacle in front (considering capsule size)
            if (HasObstacleInPath(direction, step))
            {
                break; // stop dash early if blocked
            }

            // Move via transform
            PlayerManager.instance.PlayerOrigin.transform.position += move;

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        //actionMediator.immuneInterpolation = false;

        //if (actionMediator.IsGrounded())
        //    actionMediator.DisablePhysicalMotion(0f, true);
        //else
        //    actionMediator.DisablePhysicalMotionOnLand(true);

        dashCoroutine = null;
    }


    private bool HasObstacleInPath(Vector3 dir, float checkDistance)
    {
        // Get capsule points based on your collider size
        Vector3 point1 = rb.position + Vector3.up * (bodyCollider.radius);
        Vector3 point2 = rb.position + Vector3.up * (bodyCollider.height - bodyCollider.radius);

        return Physics.CapsuleCast(
            point1,
            point2,
            bodyCollider.radius,
            dir,
            out _,
            checkDistance + 0.05f, // extra buffer
            obstacleLayerMask
        );
    }

    //private void HandleTimeModification(float modifier)
    //{
    //    if (dashCoroutine != null)
    //        actionMediator.rb.interpolation = RigidbodyInterpolation.None;
    //}

    protected override void OnDestroy()
    {
        base.OnDestroy();

        inputManager.B.action.performed -= Dash;
    }
}