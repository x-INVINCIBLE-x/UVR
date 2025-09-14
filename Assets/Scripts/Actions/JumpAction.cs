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

    private Coroutine jumpCoroutine;

    protected override void Start()
    {
        base.Start();
        inputManager.A.action.performed += HandleJump;
        actionMediator.OnTimeModified += HandleTimeChange;

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
        
        while (elapsed < jumpDuration)
        {
            if (actionMediator.grabStatus.IsClimbing())
                yield break;

            float t = elapsed / jumpDuration;
            float height = 4f * jumpHeight * t * (1 - t);

            float deltaHeight = height - previousMove.y;

            // Instead of controller.Move, directly change Y position
            Vector3 pos = PlayerManager.instance.PlayerOrigin.transform.position;
            pos.y += deltaHeight;
            PlayerManager.instance.PlayerOrigin.transform.position = pos;

            previousMove.y = height;

            // Don't auto-stop in air for multiple jumps
            if (elapsed > 0.2f && actionMediator.IsGrounded())
            {
                actionMediator.playerGravity.EnableGravity();
                currentJumpCount = jumpCount; // reset on landing
                yield break;
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

        jumpCoroutine = null;
    }


    private IEnumerator AcceleratedJumpRoutine(float jumpAcceleration)
    {
        actionMediator.immuneInterpolation = true;
        actionMediator.playerGravity.DisableGravity();
        previousMove = Vector3.zero;

        // Initial upward velocity (based on jumpAcceleration and jumpHeight)
        float velocity = Mathf.Sqrt(2f * jumpAcceleration * jumpHeight * acceleratedJumpMultiplier);

        while (velocity > 0f && !actionMediator.IsGrounded()) // Changed OR to AND
        {
            // Apply upward movement if velocity > 0
            if (velocity > 0f)
            {
                float deltaHeight = velocity * Time.deltaTime;
                Vector3 move = Vector3.up * deltaHeight;

                if (actionMediator.controller.enabled)
                    actionMediator.controller.Move(move);

                previousMove.y += deltaHeight;
            }

            // Update velocity (gravity effect)
            velocity += Physics.gravity.y * Time.deltaTime;

            // Landing check (after initial lift-off)
            if (previousMove.y > 0.2f && actionMediator.IsGrounded())
            {
                actionMediator.playerGravity.EnableGravity();
                currentJumpCount = jumpCount; // reset on landing
                break;
            }

            yield return null;
        }

        // Restore gravity fully at end
        actionMediator.immuneInterpolation = false;
        actionMediator.playerGravity.EnableGravity();
        previousMove = Vector3.zero;
        jumpCoroutine = null;
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
