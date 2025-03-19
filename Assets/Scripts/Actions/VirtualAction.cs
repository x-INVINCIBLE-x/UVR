using System;
using UnityEngine;

public class VirtualAction : MonoBehaviour
{
    public GameObject targetObject;
    public Transform followTransform;
    private Vector3 lastFollowPos;
    public float moveSpeed = 3f;
    public float threshold = 0.01f; 
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;
    private Vector2 moveInput;
    private Rigidbody targetRb;

    private void Start()
    {
        targetRb = targetObject.GetComponent<Rigidbody>();
        InputManager.Instance.leftJoystick.action.performed += OnMove;
    }

    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
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

        if (moveInput != Vector2.zero) return;

        Vector3 deltaMove = (followTransform.position - lastFollowPos);
        Debug.Log(deltaMove.sqrMagnitude);
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
