using System.Collections;
using UnityEngine;
using static UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.HapticsUtility;

public class JumpAction : Action
{
    public bool jumpGroundedOnly = true;
    public float jumpCooldown = 1f;
    public float jumpHeight = 1.5f;

    private float jumpVelocity;
    private float lastTimeJumped = -10f;
    private Vector3 previousMove;
    public float blendDuration = 0.1f;
    protected override void Start()
    {
        base.Start();
        inputManager.A.action.performed += HandleJump;
        actionMediator.OnTimeModified += HandleTimeChange;
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

        if (jumpGroundedOnly && actionMediator.IsGrounded() ||
            (!jumpGroundedOnly && lastTimeJumped + jumpCooldown < Time.time))
        {
            lastTimeJumped = Time.time;
            jumpVelocity = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);

            StartCoroutine(LerpJump());
        }
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
            float t = elapsed / jumpDuration;
            float height = 4f * jumpHeight * t * (1 - t);

            float deltaHeight = height - previousMove.y;
            Vector3 move = Vector3.up * deltaHeight;

            if (actionMediator.controller.enabled)
                actionMediator.controller.Move(move);
            previousMove.y = height;

            if (elapsed > 0.2f && actionMediator.IsGrounded())
            {
                actionMediator.playerGravity.EnableGravity();
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        actionMediator.playerGravity.SetYVelocity(-Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight) * blendDuration);
        actionMediator.immuneInterpolation = false;
        actionMediator.playerGravity.EnableGravity();
        previousMove = Vector3.zero;
    }


    private void SetPhysicalMovement()
    {
        if (Time.timeScale == 1)
        {
            actionMediator.SetPhysicalMotion(true);
        }
        else
        {
            actionMediator.immuneInterpolation = true;
            actionMediator.SetPhysicalMotion(true, true);
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
