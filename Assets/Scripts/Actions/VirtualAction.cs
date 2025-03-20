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
    public Transform followTransform;
    private Vector3 lastFollowPos;
    public float moveSpeed = 3f;
    public float threshold = 0.01f; 
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;
    private Vector2 moveInput;
    private Rigidbody targetRb;

    public bool isMoving = false;

    private void Start()
    {
        InputManager.Instance.leftJoystick.action.performed += OnMove;
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

    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        isMoving = !Mathf.Approximately(moveInput.x, 0) || !Mathf.Approximately(moveInput.x, 0);
        Debug.Log(moveInput.x + "   " + moveInput.y);
    }

    public void SetTargetObject(GameObject ob)
    {
        targetObject = ob;
        lastFollowPos = followTransform.position;

        if (targetObject.TryGetComponent(out targetRb))
        {
            targetRb.isKinematic = false; 
            targetRb.useGravity = false;  
            targetRb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    private void FixedUpdate() 
    {
        if (targetObject == null || targetRb == null) return;

        if (isMoving) return;

        Vector3 deltaMove = (followTransform.position - lastFollowPos);

        if (deltaMove.sqrMagnitude < threshold)
        {
            targetRb.linearVelocity = Vector3.zero;
            return;
        }

        lastFollowPos = followTransform.position; 

        Vector3 targetPos = Vector3.SmoothDamp(
            targetObject.transform.position,
            targetObject.transform.position + deltaMove * moveSpeed,
            ref velocity,
            smoothTime
        );

        targetRb.MovePosition(targetPos);
    }

    public void DeselectTargetObject(GameObject ob)
    {
        if (targetObject != ob) return;

        if (targetObject.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true; 
            rb.useGravity = true;  
        }

        targetObject = null;
    }
}
