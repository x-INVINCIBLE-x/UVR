using System.Collections;
using UnityEngine;
using static UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.HapticsUtility;

public class JumpAction : Action
{
    public bool jumpGroundedOnly = true;
    public float jumpCooldown = 1f;
    public float jumpHeight = 1.5f;

    private float lastTimeJumped = -10f;
    private Vector3 previousMove;
    public float blendDuration = 0.1f;

    public int jumpCount = 1;
    [Min(1)]
    public float acceleratedJumpMultiplier = 1f;
    private int currentJumpCount;
    private bool jumpStarted = false;

    [SerializeField] private bool changeFOV = true;
    [SerializeField] private int jumpFOV = 65;
    [SerializeField] private float fovChangeDuration = 0.25f;
    [SerializeField] private AnimationCurve fovCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float defaultFOV;
    private Coroutine fovCoroutine;
    private Coroutine jumpCoroutine;
    private Coroutine resetRoutine;

    protected override void Start()
    {
        base.Start();
        inputManager.A.action.performed += HandleJump;
        actionMediator.OnTimeModified += HandleTimeChange;

        defaultFOV = Camera.main.fieldOfView;
        currentJumpCount = jumpCount;
    }

    private void HandleTimeChange(float obj)
    {
        //if (actionMediator.IsGrounded()) return;
        //actionMediator.rb.interpolation = RigidbodyInterpolation.Interpolate;
        //actionMediator.immuneInterpolation = true;
    }

    private void HandleJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!isPermitted) return;
        AudioManager.Instance.PlaySFX2d(actionMediator.audioSource, sfxClip, 1);
        if (actionMediator.IsGrounded())
        {
            currentJumpCount = jumpCount;
        }

        if (currentJumpCount > 0 && lastTimeJumped + jumpCooldown < Time.time)
        {
            lastTimeJumped = Time.time;
            currentJumpCount--;

            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
                jumpCoroutine = null;
            }

            jumpCoroutine = StartCoroutine(LerpJump());
        }
    }

    public void AcceleratedJump(float acceleration)
    {
        if (acceleration == 0f) return;

        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
        }

        StartCoroutine(AcceleratedJumpRoutine(acceleration));
    }
    private IEnumerator LerpJump()
    {
        float jumpDuration = 2f * Mathf.Sqrt(2f * jumpHeight / -Physics.gravity.y);
        float elapsed = 0f;

        actionMediator.immuneInterpolation = true;
        actionMediator.playerGravity.DisableGravity();
        previousMove = Vector3.zero;

        if (changeFOV)
        {
            if (fovCoroutine != null)
                StopCoroutine(fovCoroutine);
        }

        while (elapsed < jumpDuration)
        {
            if (actionMediator.grabStatus.IsClimbing())
                yield break;

            float t = elapsed / jumpDuration;
            float height = 4f * jumpHeight * t * (1 - t);
            float deltaHeight = height - previousMove.y;

            Vector3 pos = PlayerManager.instance.PlayerOrigin.transform.position;
            pos.y += deltaHeight;
            PlayerManager.instance.PlayerOrigin.transform.position = pos;

            previousMove.y = height;

            if (elapsed > 0.2f && actionMediator.IsGrounded())
            {
                actionMediator.playerGravity.EnableGravity();
                currentJumpCount = jumpCount;
                break;
            }

            if (changeFOV)
            {
                float curveValue = fovCurve.Evaluate(t); 
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, jumpFOV, curveValue);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

      
        actionMediator.playerGravity.SetYVelocity(
            -Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight) * blendDuration
        );
        actionMediator.immuneInterpolation = false;
        actionMediator.playerGravity.EnableGravity();
        previousMove = Vector3.zero;

        if (changeFOV)
        {
            if (fovCoroutine != null)
                StopCoroutine(fovCoroutine);

            fovCoroutine = StartCoroutine(ResetFOVRoutine(fovChangeDuration));
        }

        jumpCoroutine = null;
    }

    private IEnumerator AcceleratedJumpRoutine(float jumpAcceleration)
    {
        actionMediator.immuneInterpolation = true;
        actionMediator.playerGravity.DisableGravity();
        previousMove = Vector3.zero;

        float velocity = Mathf.Sqrt(2f * jumpAcceleration * jumpHeight * acceleratedJumpMultiplier);
        float totalDuration = (2f * velocity) / -Physics.gravity.y;
        float elapsed = 0f;
        if (changeFOV)
        {
            if (fovCoroutine != null)
                StopCoroutine(fovCoroutine);
        }

        while (velocity > 0f && !actionMediator.IsGrounded())
        {
            float deltaHeight = velocity * Time.deltaTime;
            Vector3 move = Vector3.up * deltaHeight;

            if (actionMediator.controller.enabled)
                actionMediator.controller.Move(move);

            previousMove.y += deltaHeight;
            velocity += Physics.gravity.y * Time.deltaTime;

            if (previousMove.y > 0.2f && actionMediator.IsGrounded())
            {
                actionMediator.playerGravity.EnableGravity();
                currentJumpCount = jumpCount;
                break;
            }

            if (changeFOV)
            {
                float t = Mathf.Clamp01(elapsed / totalDuration);
                float curveValue = fovCurve.Evaluate(t);
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, jumpFOV, curveValue);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        actionMediator.immuneInterpolation = false;
        actionMediator.playerGravity.EnableGravity();
        previousMove = Vector3.zero;
        jumpCoroutine = null;

        if (changeFOV)
        {
            if (fovCoroutine != null)
                StopCoroutine(fovCoroutine);

            fovCoroutine = StartCoroutine(ResetFOVRoutine(fovChangeDuration));
        }
    }

    private IEnumerator ResetFOVRoutine(float duration = 0.25f)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = fovCurve.Evaluate(t);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defaultFOV, curveValue);
            yield return null;
        }

        Camera.main.fieldOfView = defaultFOV;
        fovCoroutine = null;
    }

    public void AddJumps(int amt) => jumpCount += amt;

    public void RemoveJumps(int amt)
    {
        jumpCount = Mathf.Min(2, amt);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        inputManager.A.action.performed -= HandleJump;
    }
}
