using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
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
    private Coroutine resetRoutine;
    private Vector2 input;

    private Vector3 heightOffset = new Vector3(0f, 0.5f, 0f);
    public bool changeFOV = true;
    private float defaultFOV;
    public int dashFOV = 61;

    [SerializeField] private AnimationCurve fovCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);

    protected override void Start()
    {
        base.Start();

        defaultDashForce = dashForce;
        defaultFOV = Camera.main.fieldOfView;
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
        StartAbility();

        Vector3 dashVelocity = direction * dashForce;
        float step = dashVelocity.magnitude * Time.unscaledDeltaTime;
        if (HasObstacleInPath(direction, step)) return;

        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }

        dashCoroutine = StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        float timer = 0f;

        // Compute dash direction once
        direction = headTransform.right * input.x + headTransform.forward * input.y;
        direction.y = 0f;
        direction.Normalize();

        Vector3 dashVelocity = direction * dashForce;

        float baseFOV = defaultFOV;

        if (resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
        }

        // DASH LOOP
        while (timer < dashDuration)
        {
            // Distance to move this frame
            float step = dashVelocity.magnitude * Time.unscaledDeltaTime;
            Vector3 move = direction * step;

            // Stop if obstacle detected
            if (HasObstacleInPath(direction, step))
                break;

            // Move player
            PlayerManager.instance.PlayerOrigin.transform.position += move;

            // FOV change (increase + decrease during dash)
            if (changeFOV)
            {
                float t = Mathf.Clamp01(timer / dashDuration);
                float curveValue = Mathf.Sin(t * Mathf.PI); 
                Camera.main.fieldOfView = Mathf.Lerp(baseFOV, dashFOV, curveValue);
            }

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // Ensure final reset
        if (changeFOV)
        {
            resetRoutine = StartCoroutine(SmoothResetFOV(baseFOV));
            yield return resetRoutine;
        }

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

    private IEnumerator SmoothResetFOV(float targetFOV, float duration = 0.25f)
    {
        float startFOV = Camera.main.fieldOfView;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            Camera.main.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            yield return null;
        }

        Camera.main.fieldOfView = targetFOV; // final clamp
        resetRoutine = null;
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