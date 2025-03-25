using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VirtualAction : Action
{
    public InputActionProperty moveAction;
    public XRGazeInteractor gazeInteractor;
    public GameObject targetObject;
    public Transform handTransform;
    public Transform bodyTransform;
    private Vector3 lastFollowPos;
    public float moveSpeed = 3f;
    public float threshold = 0.01f;
    private float effectiveSpeed;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;
    private Vector2 moveInput;
    private Rigidbody targetRb;
    private Transform followTransform;

    private bool isMoving = false;
    private bool lastMovingStatus = false;
    private float defaultMoveSpeed;
    protected override void Start()
    {
        base.Start();
        effectiveSpeed = moveSpeed;
        inputManager.leftJoystick.action.performed += OnMove;
        moveAction.action.performed += OnMove;
        gazeInteractor.selectEntered.AddListener(SelectObject);
    }

    private void SelectObject(SelectEnterEventArgs args)
    {
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
            DeselectTargetObject(targetObject);
        }
    }

    public void SetTargetObject(GameObject ob)
    {
        targetObject = ob;
        lastFollowPos = handTransform.position;

        if (targetObject.TryGetComponent(out targetRb))
        {
            targetRb.isKinematic = false; 
            targetRb.useGravity = false;  
            targetRb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (targetObject == null) { return; }

        moveInput = context.ReadValue<Vector2>();

        isMoving = !Mathf.Approximately(moveInput.x, 0) || !Mathf.Approximately(moveInput.y, 0);


        Debug.Log(moveInput.x + "   " + moveInput.y);
    }

    private void Update() 
    {
        if (targetObject == null || targetRb == null) return;

        // Determine the current follow transform
        //Transform newFollowTransform = isMoving ? bodyTransform : handTransform;
        Transform newFollowTransform = handTransform;
        
        // If switching transforms, reset lastFollowPos to prevent sudden jumps
        //if (newFollowTransform != followTransform)
        //{
            //lastFollowPos = newFollowTransform.position;
            if (isMoving)
                effectiveSpeed = moveSpeed / 20;
            else
                effectiveSpeed = moveSpeed;
        //}

        followTransform = newFollowTransform;

        Vector3 deltaMove = (followTransform.position - lastFollowPos);

        if (deltaMove.sqrMagnitude < threshold)
        {
            targetRb.linearVelocity = Vector3.zero;
            return;
        }

        lastFollowPos = followTransform.position;

        Vector3 targetPos = Vector3.SmoothDamp(
            targetObject.transform.position,
            targetObject.transform.position + deltaMove * effectiveSpeed,
            ref velocity,
            smoothTime
        );

        targetRb.MovePosition(targetPos);
    }

    public void DeselectTargetObject(GameObject ob)
    {
        if (targetObject.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false; 
            rb.useGravity = true;  
        }

        targetObject = null;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        inputManager.leftJoystick.action.performed -= OnMove;
        moveAction.action.performed -= OnMove;
        gazeInteractor.selectEntered.RemoveListener(SelectObject);
    }
}
