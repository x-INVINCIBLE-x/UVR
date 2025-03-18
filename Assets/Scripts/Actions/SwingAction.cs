using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwingAction : MonoBehaviour
{
    private ActionMediator actionMediator;

    public Transform startSwingPoint;
    public float maxDistance;
    public LayerMask swingLayer;
    public float swingRadius;

    public Transform predictionPoint;

    public InputActionProperty swingAction;
    public InputActionProperty pullAction;

    public float pullingStrngth = 500f;

    public LineRenderer lineRenderer;
    public SpringJoint joint;

    private Vector3 swingPoint;
    private bool hasHit;

    private void Awake()
    {
        actionMediator = GetComponentInParent<ActionMediator>();
    }

    private void Update()
    {
        GetSwingPoint();

        if (swingAction.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.P))
        {
            StartSwing();
        }
        else if (swingAction.action.WasReleasedThisFrame() || Input.GetKeyUp(KeyCode.P))
        {
            StopSwing();
        }

        PullRope();
        DrawRope();
    }

    private void StartSwing()
    {
        if (!hasHit) return;
        Debug.Log("enter");

        actionMediator.SetPhysicalMotion(true);

        joint = actionMediator.rb.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distance = Vector3.Distance(actionMediator.rb.position, swingPoint);
        joint.maxDistance = distance;

        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;
    }

    private void StopSwing()
    {
        actionMediator.DisablePhysicalMotionOnLand();

        Debug.Log("Destroy");
        Destroy(joint);
    }

    public void PullRope()
    {
        if (!joint) return;

        if (!pullAction.action.IsPressed()) return;
        Debug.Log("pull");
        Vector3 direction = (swingPoint - startSwingPoint.position).normalized;
        actionMediator.rb.AddForce(direction * pullingStrngth * Time.deltaTime);

        float distance = Vector3.Distance(actionMediator.rb.position, swingPoint);
        joint.maxDistance = distance;
    }

    private void GetSwingPoint()
    {
        if (joint)
        {
            predictionPoint.gameObject.SetActive(false);
            return;
        }

        RaycastHit raycastHit;

        hasHit = Physics.SphereCast(startSwingPoint.position, swingRadius, startSwingPoint.forward, out raycastHit, maxDistance, swingLayer);
        //hasHit = Physics.Raycast(startSwingPoint.position, startSwingPoint.forward, out raycastHit, maxDistance, swingLayer);

        if (hasHit)
        {
            swingPoint = raycastHit.point;
            predictionPoint.position = swingPoint;
            predictionPoint.gameObject.SetActive(true);
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }
    }

    private void DrawRope()
    {
        if (joint)
        {
        Debug.Log("rope");
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startSwingPoint.position);
            lineRenderer.SetPosition(1, swingPoint);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}
