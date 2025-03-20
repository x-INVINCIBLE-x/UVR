using UnityEngine;

public class DashAction : MonoBehaviour
{
    public ActionMediator actionMediator;
    public Rigidbody rb;

    public float dashForce;
    public float dashDuration = 0.5f;
    public Transform headTransform;

    public void Start()
    {
        actionMediator = GetComponent<ActionMediator>();
        InputManager.Instance.B.action.performed += Dash;
        rb = actionMediator.rb;
    }

    private void Dash(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        actionMediator.SetPhysicalMotion(true);
        rb.AddForce(headTransform.forward *  dashForce, ForceMode.VelocityChange);
        actionMediator.DisablePhysicalMotion(dashDuration);
    }
}