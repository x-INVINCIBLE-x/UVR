using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwingAction : MonoBehaviour
{
    [SerializeField] private ActionMediator actionMediator;

    public Hand hand; 
    public Transform startSwingPoint;
    public float maxDistance;
    public LayerMask swingLayer;
    public float swingRadius;

    public Transform predictionPoint;

    public InputActionProperty swingAction;
    public InputActionProperty pullAction;

    public float pullingStrength;

    public LineRenderer lineRenderer;
    public SpringJoint joint;

    private Vector3 swingPoint;
    private float defaultPullingStrength;
    private bool hasHit;

    private void Awake()
    {
        //actionMediator = GetComponentInParent<ActionMediator>();
    }

    private void Start()
    {
        actionMediator.OnTimeModified += HandleTimeChange;

        defaultPullingStrength = pullingStrength;
    }

    private void HandleTimeChange(float timeMultiplier)
    {
        //if (timeMultiplier != 1)
        //{
        //    pullingStrength /= timeMultiplier;
        //}
        //else
        //{
        //    pullingStrength = defaultPullingStrength;
        //}
    }

    private void Update()
    {
        GetSwingPoint();

        if (swingAction.action.WasPressedThisFrame())
        {
            if (actionMediator.grabStatus.GetStatus(hand) != GrabType.Empty)
                return;

            StartSwing();
        }
        else if (swingAction.action.WasReleasedThisFrame())
        {
            StopSwing();
        }

        PullRope();
        DrawRope();
    }

    private void StartSwing()
    {
        if (!hasHit) return;

        //actionMediator.immuneInterpolation = true;
        //actionMediator.

        actionMediator.grabStatus.ChangeHandStatus(hand, GrabType.Swing);
        actionMediator.SetPhysicalMotion(true);

        actionMediator.turnProvider.enabled = false;
        actionMediator.rbTurnProvider.enabled = true;

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
        actionMediator.grabStatus.ResetHandStatus(hand);

        if (!actionMediator.grabStatus.IsSwinging())
        {
            actionMediator.DisablePhysicalMotionOnLand();
            //actionMediator.turnProvider.enabled = true;
            //actionMediator.rbTurnProvider.enabled = false;
        }

        Destroy(joint);
    }

    public void PullRope()
    {
        if (!joint) return;

        if (actionMediator.grabStatus.IsClimbing()) { return; }

        if (!pullAction.action.IsPressed()) return;
        
        Vector3 direction = (swingPoint - startSwingPoint.position).normalized;
        actionMediator.rb.AddForce(direction * pullingStrength * Time.unscaledDeltaTime);

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
