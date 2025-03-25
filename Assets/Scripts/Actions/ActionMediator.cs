using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

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

    public GrabStatus grabStatus;
    public Rigidbody rb;
    public CharacterController controller;
    public GameObject bodyCollider;
    public LayerMask whatIsGround;
    public Coroutine landRoutine = null;

    public float groundCheckOffset = 0.2f;
    public event Action<float> OnTimeModified;

    public ActionStatus LastActionStatus {  get; private set; } = ActionStatus.None;

    private void Awake()
    {
        grabStatus = GetComponent<GrabStatus>();
        grabStatus.GrabStatusChanged += HandleGrabStatusChange;
    }

    private void HandleGrabStatusChange(GrabType type)
    {
        if (!grabStatus.IsClimbing())
        {
            if (LastActionStatus == ActionStatus.Climb)
            {
                SetPhysicalMotion(true);
                DisablePhysicalMotionOnLand();
            }
        }

        if (grabStatus.IsClimbing())
        {
            LastActionStatus = ActionStatus.Climb;
            SetPhysicalMotion(false);
            if (landRoutine != null)
                StopCoroutine(landRoutine);
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

    public void SetPhysicalMotion(bool status)
    {
        rb.isKinematic = !status;
        controller.enabled = !status;
        bodyCollider.SetActive(status);
        xRBodyTransformer.useCharacterControllerIfExists = !status;
        //rb.interpolation = status ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
    }

    public void DisablePhysicalMotion(float duration) => StartCoroutine(DisablePhysicalMotionRoutine(duration));

    public void DisablePhysicalMotionOnLand()
    {
        landRoutine ??= StartCoroutine(LandRoutine());
    }

    IEnumerator DisablePhysicalMotionRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetPhysicalMotion(false);
    }

    IEnumerator LandRoutine()
    {
        //float safteyTimer = 5;
        yield return new WaitForSeconds(0.1f);
        while (!IsGrounded())
        {
            //safteyTimer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        SetPhysicalMotion(false);
        landRoutine = null;
    }

    public void TimeScaleUpdated(float timeModifier) => OnTimeModified?.Invoke(timeModifier);

    public bool IsGrounded()
    {
        Vector3 start = controller.transform.TransformPoint(controller.center);
        float rayLength = controller.height / 2 - controller.radius + groundCheckOffset;
        bool hasHit = Physics.SphereCast(start, controller.radius, Vector3.down, out RaycastHit _, rayLength, whatIsGround);
        Debug.Log("IS Grounded: " + hasHit);
        return hasHit;
    }

    private void OnDisable()
    {
        grabStatus.GrabStatusChanged -= HandleGrabStatusChange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 start = controller.transform.TransformPoint(controller.center);
        float rayLength = controller.height / 2 - controller.radius + groundCheckOffset;
        Gizmos.DrawRay(start, Vector3.down * rayLength);
    }
}