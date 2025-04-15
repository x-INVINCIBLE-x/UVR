using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public enum ActionStatus
{
    None,
    Climb,
    Swing
}

public class ActionMediator : MonoBehaviour
{
    //public GameObject abilitySelectDisplay;
    public XRBodyTransformer xRBodyTransformer;
    public DynamicMoveProvider moveProvider;
    private PlayerGravity playerGravity;

    public GrabStatus grabStatus;
    public Rigidbody rb;
    public CharacterController controller;
    public GameObject bodyCollider;
    public LayerMask whatIsGround;
    public Coroutine landRoutine = null;

    public float groundCheckOffset = 0.2f;
    public event Action<float> OnTimeModified;
    private float defaultMoveSpeed = 0;

    [field: SerializeField] public ActionStatus LastActionStatus { get; private set; } = ActionStatus.None;
    // ------------ Changes -------------------

    public class ConstantVector2InputReader : IXRInputValueReader<Vector2>
    {
        private Vector2 _value;

        public ConstantVector2InputReader(Vector2 value)
        {
            _value = value;
        }
        public Vector2 ReadValue()
        {
            return _value;
        }

        public bool TryReadValue(out Vector2 value)
        {
            value = _value;
            return true;
        }
    }

    private void Awake()
    {
        grabStatus = GetComponent<GrabStatus>();
        playerGravity = GetComponent<PlayerGravity>();
        //grabStatus.GrabStatusChanged += HandleGrabStatusChange;
    }

    private void OnEnable()
    {
        // ----------------- Changes: From Awake -> OnEnable ---------------------------------------------
        grabStatus.GrabStatusChanged += HandleGrabStatusChange;
        OnTimeModified += HandleTimeChange;
    }

    private void HandleGrabStatusChange(GrabType type)
    {
        if (!grabStatus.IsClimbing())
        {
            if (LastActionStatus == ActionStatus.Climb)
            {
                //SetPhysicalMotion(true);
                playerGravity.EnableGravity();
                EnableMovement();
                //DisablePhysicalMotionOnLand();
            }
        }

        if (grabStatus.IsClimbing())
        {
            LastActionStatus = ActionStatus.Climb;
            playerGravity.DisableGravity();
            DisableMovement();
            //SetPhysicalMotion(false);
            //if (landRoutine != null)
            //    StopCoroutine(landRoutine);
        }
        else if (grabStatus.IsSwinging())
        {
            LastActionStatus = ActionStatus.Swing;
        }
        else
        {
            LastActionStatus = ActionStatus.None;
        }
    }

    protected virtual void Start()
    {
        SetPhysicalMotion(false);
        //InputManager.Instance.leftJoystickPress.action.performed += ToggleAbilitySelect;
    }

    //private void ToggleAbilitySelect(UnityEngine.InputSystem.InputAction.CallbackContext context)
    //{
    //    abilitySelectDisplay.SetActive(!abilitySelectDisplay.activeSelf);
    //}

    public void EnableMovement()
    {
        moveProvider.leftHandMoveInput.bypass = null;
        moveProvider.rightHandMoveInput.bypass = null;
    }

    public void DisableMovement()
    {
        //moveProvider.leftHandMoveInput.manualValue = Vector2.zero;
        //moveProvider.rightHandMoveInput.manualValue = Vector2.zero;

        //moveProvider.enabled = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        moveProvider.leftHandMoveInput.bypass = new ConstantVector2InputReader(Vector2.zero);
        moveProvider.rightHandMoveInput.bypass = new ConstantVector2InputReader(Vector2.zero);
    }

    public void SetPhysicalMotion(bool status, bool interpolate = false)
    {
        xRBodyTransformer.useCharacterControllerIfExists = !status;
        rb.isKinematic = !status;
        controller.enabled = !status;
        bodyCollider.SetActive(status);

        if (interpolate && status)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        else if (interpolate && !status)
        {
            rb.interpolation = RigidbodyInterpolation.None;
        }

        //rb.interpolation = status ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
    }

    public void DisablePhysicalMotion(float duration, bool interpolate = false) => StartCoroutine(DisablePhysicalMotionRoutine(duration, interpolate));

    public void DisablePhysicalMotionOnLand(bool interpolate = false)
    {
        landRoutine ??= StartCoroutine(LandRoutine(interpolate));
    }

    IEnumerator DisablePhysicalMotionRoutine(float delay, bool interpolate = false)
    {
        yield return new WaitForSeconds(delay);
        SetPhysicalMotion(false);
    }

    IEnumerator LandRoutine(bool interpolate)
    {
        //float safteyTimer = 5;
        yield return new WaitForSeconds(0.1f);
        while (!IsGrounded())
        {
            //safteyTimer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        SetPhysicalMotion(false, interpolate);
        landRoutine = null;
    }

    private void HandleTimeChange(float timeScale) => StartCoroutine(TimeChangeCoroutine()); 

    private IEnumerator TimeChangeCoroutine()
    {
        while (Time.timeScale != 1)
        {
            if (!playerGravity.IsGrounded() && rb.interpolation != RigidbodyInterpolation.Interpolate) 
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            yield return new WaitForEndOfFrame();
            if (playerGravity.IsGrounded() && rb.interpolation == RigidbodyInterpolation.Interpolate)
                rb.interpolation = RigidbodyInterpolation.None;
        }

        rb.interpolation = RigidbodyInterpolation.None;
    }

    public void TimeScaleUpdated(float timeModifier) => OnTimeModified?.Invoke(timeModifier);

    public bool IsGrounded()
    {
        Vector3 start = controller.transform.TransformPoint(controller.center);
        float rayLength = controller.height / 2 - controller.radius + groundCheckOffset;
        bool hasHit = Physics.SphereCast(start, controller.radius, Vector3.down, out RaycastHit _, rayLength, whatIsGround);
        return hasHit;
    }

    private void OnDisable()
    {
        grabStatus.GrabStatusChanged -= HandleGrabStatusChange;
        OnTimeModified -= HandleTimeChange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 start = controller.transform.TransformPoint(controller.center);
        float rayLength = controller.height / 2 - controller.radius + groundCheckOffset;
        Gizmos.DrawRay(start, Vector3.down * rayLength);
    }
}