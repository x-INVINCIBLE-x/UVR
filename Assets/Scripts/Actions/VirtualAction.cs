using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VirtualAction : Action
{
    public InputActionProperty moveAction;
    public XRAdvanceGazeInteractor gazeInteractor;
    public GameObject targetObject;
    public Transform handTransform;
    public Transform bodyTransform;
    private Vector3 lastFollowPos;

    public float moveSpeed = 3f;
    public float threshold = 0.01f;
    private float effectiveSpeed;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;
    private Rigidbody targetRb;

    //Track Player Movement
    [Header("Track Movement")]
    public Transform trackingTransform; 
    public float movementThreshold = 0.01f;
    public float movingDivisor = 15f;

    private Vector3 lastPosition;
    public bool isMoving;

    [Header("Tracked Object Rigidbody Status")]
    private bool isKinematic;
    private bool hasGravity;

    protected override void Start()
    {
        base.Start();
        effectiveSpeed = moveSpeed;
        inputManager.leftJoystick.action.performed += OnMove;
        moveAction.action.performed += OnMove;
        gazeInteractor.selectEntered.AddListener(SelectObject);

        if (trackingTransform == null)
            trackingTransform = Camera.main.transform; 

        lastPosition = trackingTransform.position;
        gazeInteractor.showInteraction = isPermitted;
    }

    private void Update()
    {
        if (!isPermitted) return;
        if (targetObject == null || targetRb == null) return;

        CheckMovement();

        if (isMoving)
            effectiveSpeed = moveSpeed / movingDivisor;
        else
            effectiveSpeed = moveSpeed;

        Vector3 deltaMove = (handTransform.position - lastFollowPos);

        if (deltaMove.sqrMagnitude < threshold)
        {
            targetRb.linearVelocity = Vector3.zero;
            return;
        }

        lastFollowPos = handTransform.position;

        Vector3 targetPos = Vector3.SmoothDamp(
            targetObject.transform.position,
            targetObject.transform.position + deltaMove * effectiveSpeed,
            ref velocity,
            smoothTime
        );

        targetRb.MovePosition(targetPos);
    }

    public override void PermitAbility(bool status)
    {
        base.PermitAbility(status);

        gazeInteractor.showInteraction = status;
    }

    private void CheckMovement()
    {
        Vector3 currentPosition = trackingTransform.position;
        float distanceMoved = Vector3.Distance(currentPosition, lastPosition);

        isMoving = distanceMoved > movementThreshold;

        lastPosition = currentPosition;
    }

    private void SelectObject(SelectEnterEventArgs args)
    {
        if (!isPermitted) return;

        SetTargetObject(args.interactableObject.transform.gameObject);
    }

    protected override void ExecuteAbility()
    {
        base.ExecuteAbility();
        if (targetObject == null)
        {
            // Handled by Gaze Interactor
        }
        else
        {
            DeselectTargetObject();
        }
    }

    public void SetTargetObject(GameObject ob)
    {
        if (!CanUseAbility()) return;
        Debug.Log("SelectS");

        targetObject = ob;
        lastFollowPos = handTransform.position;

        if (targetObject.TryGetComponent(out targetRb))
        {
            isKinematic = targetRb.isKinematic;
            hasGravity = targetRb.useGravity;

            targetRb.isKinematic = false;
            targetRb.useGravity = false;
        }

        gazeInteractor.showInteraction = false;
    }

    protected override void StartAbility()
    {
        ExecuteAbility();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (targetObject == null) { return; }

        // By Input
        //Vector2 moveInput = context.ReadValue<Vector2>();
        ////isMoving = !Mathf.Approximately(moveInput.x, 0) || !Mathf.Approximately(moveInput.y, 0);
        //isMoving = isMoving || (!Mathf.Approximately(moveInput.x, 0) && !Mathf.Approximately(moveInput.y, 0));
        Debug.Log("Moving Status: " + isMoving);
    }

    public void DeselectTargetObject()
    {
        lastTimeSkillUsed = Time.time;
        if (targetObject.TryGetComponent(out Rigidbody _))
        {
            targetRb.isKinematic = isKinematic;
            targetRb.useGravity = hasGravity;
        }
        Debug.Log("Deselect");
        targetObject = null;

        gazeInteractor.showInteraction = true;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        inputManager.leftJoystick.action.performed -= OnMove;
        moveAction.action.performed -= OnMove;
        gazeInteractor.selectEntered.RemoveListener(SelectObject);
    }
}
