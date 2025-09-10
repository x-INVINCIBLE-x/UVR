using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
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
    public ContinuousTurnProvider turnProvider;
    public ManualRigidbodyTurn rbTurnProvider;
    public PlayerGravity playerGravity;
    public VelocityEstimator playerVelocity;

    public GrabStatus grabStatus;
    public Rigidbody rb;
    public CharacterController controller;
    public GameObject bodyCollider;
    public LayerMask whatIsGround;
    public Coroutine landRoutine = null;

    public float groundCheckOffset = 0.2f;
    public event Action<float> OnTimeModified;
    public bool immuneInterpolation = false;
    public bool defaultKinematicStatus = true;

    public TimeAction TimeAction;
    public JumpAction JumpAction;
    public VirtualAction TelekemisisAction;
    public DashAction DashAction;

    [field: SerializeField] public ActionStatus LastActionStatus { get; private set; } = ActionStatus.None;

    private float defaultSpeed;
    [SerializeField] private float rotationFixDuration = 1f;
    private Coroutine rotationFixRoutine = null;
    [SerializeField] private List<float> speedMultipliers = new List<float>();

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
        defaultSpeed = moveProvider.moveSpeed;
        //grabStatus.GrabStatusChanged += HandleGrabStatusChange;
    }

    private void OnEnable()
    {
        // ----------------- Changes: From Awake -> OnEnable ---------------------------------------------
        grabStatus.GrabStatusChanged += HandleGrabStatusChange;
        OnTimeModified += HandleTimeChange;
    }

    protected virtual void Start()
    {
        SetPhysicalMotion(false);
    }

    private void HandleGrabStatusChange(GrabType type)
    {
        if (!grabStatus.IsClimbing())
        {
            if (LastActionStatus == ActionStatus.Climb)
            {
                //SetPhysicalMotion(true);
                //Debug.Log("Velocity: " + playerVelocity.GetAccelerationEstimate());
                playerGravity.EnableGravity();
                EnableMovement();
                rb.isKinematic = defaultKinematicStatus;
                
                //if (JumpAction != null)
                //    JumpAction.AcceleratedJump(playerVelocity.GetAccelerationEstimate().magnitude);

                //DisablePhysicalMotionOnLand();
            }
        }

        if (!grabStatus.IsSwinging())
        {
            if (LastActionStatus == ActionStatus.Swing && rotationFixRoutine == null)
            {
                rotationFixRoutine = StartCoroutine(RotateTo(rb.transform, Vector3.zero, rotationFixDuration));
            }
        }

        if (grabStatus.IsClimbing())
        {
            LastActionStatus = ActionStatus.Climb;
            playerGravity.DisableGravity();
            DisableMovement();
            defaultKinematicStatus = rb.isKinematic;
            rb.isKinematic = true;
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

    #region Handles Movement
    public void AddSpeedMultiplier(float multiplier)
    {
        speedMultipliers.Add(multiplier);
        UpdateSpeed();
    }

    public void RemoveSpeedMultiplier(float multiplier)
    {
        if (speedMultipliers.Contains(multiplier))
        {
            speedMultipliers.Remove(multiplier);
            UpdateSpeed();
        }
    }

    private void UpdateSpeed()
    {
        float finalMultiplier = 1f;
        foreach (var m in speedMultipliers)
            finalMultiplier *= m;

        moveProvider.moveSpeed = defaultSpeed * finalMultiplier;
    }

    public void EnableControl()
    {
        EnableMovement();
        EnableRotationt();
    }

    public void DisableControl()
    {
        DisableMovement();
        DisableRotation();
    }

    public void EnableMovement()
    {
        moveProvider.leftHandMoveInput.bypass = null;
        moveProvider.rightHandMoveInput.bypass = null;
    }

    public void EnableRotationt()
    {
        turnProvider.leftHandTurnInput.bypass = null;
        turnProvider.rightHandTurnInput.bypass = null;
    }

    public void DisableMovement()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        moveProvider.leftHandMoveInput.bypass = new ConstantVector2InputReader(Vector2.zero);
        moveProvider.rightHandMoveInput.bypass = new ConstantVector2InputReader(Vector2.zero);
    }
    public void DisableRotation()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        turnProvider.leftHandTurnInput.bypass = new ConstantVector2InputReader(Vector2.zero);
        turnProvider.rightHandTurnInput.bypass = new ConstantVector2InputReader(Vector2.zero);
    }
    #endregion

    #region Handle Physical Motion
    public void SetPhysicalMotion(bool status, bool interpolate = false)
    {
        xRBodyTransformer.useCharacterControllerIfExists = !status;
        rb.isKinematic = !status;
        controller.enabled = !status;
        //bodyCollider.SetActive(status);

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
        SetPhysicalMotion(false, interpolate);
        immuneInterpolation = false;
    }

    IEnumerator LandRoutine(bool interpolate)
    {
        yield return new WaitForSeconds(0.1f); // ---------------- Original 0.1 - that also is a magic numbrt -----------------
        while (!IsGrounded())
        {
            yield return new WaitForFixedUpdate();
        }

        // -------------------- Swing specific ---------------------------
        turnProvider.enabled = true;
        rbTurnProvider.enabled = false;

        yield return new WaitForEndOfFrame();
        SetPhysicalMotion(false, interpolate);
        immuneInterpolation = false;
        landRoutine = null;
    }

    #endregion

    #region Handle Time Change
    private void HandleTimeChange(float timeScale) => StartCoroutine(TimeChangeCoroutine()); 

    private IEnumerator TimeChangeCoroutine()
    {
        while (Time.timeScale != 1)
        {
            if (!immuneInterpolation && rb.isKinematic && !playerGravity.IsGrounded() && rb.interpolation != RigidbodyInterpolation.Interpolate)
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            yield return new WaitForEndOfFrame();
            if (!immuneInterpolation && playerGravity.IsGrounded() && rb.interpolation == RigidbodyInterpolation.Interpolate)
                rb.interpolation = RigidbodyInterpolation.None;
        }

        //rb.interpolation = RigidbodyInterpolation.None;
    }

    public void TimeScaleUpdated(float timeModifier) => OnTimeModified?.Invoke(timeModifier);

    #endregion

    private IEnumerator RotateTo(Transform transform, Vector3 targetEulerAngles, float duration)
    {
        Quaternion startRotation = transform.rotation;

        Vector3 currentEuler = transform.eulerAngles;
        Vector3 targetEuler = new Vector3(0f, currentEuler.y, 0f);
        Quaternion endRotation = Quaternion.Euler(targetEuler);

        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            // You can use Quaternion.Slerp here too for cleaner rotation
            float x = Mathf.LerpAngle(currentEuler.x, 0f, t);
            float y = currentEuler.y;
            float z = Mathf.LerpAngle(currentEuler.z, 0f, t);
            transform.rotation = Quaternion.Euler(x, y, z);

            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, currentEuler.y, 0f);
        rotationFixRoutine = null;
    }


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