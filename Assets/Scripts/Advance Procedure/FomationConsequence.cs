using UnityEngine;

public abstract class FomationConsequence : MonoBehaviour
{
    [SerializeField] private DynamicFormationController cubeFormationController;
    [SerializeField] protected FormationType type;

    protected virtual void Start()
    {
        cubeFormationController.OnFormationStart += HandleFormationStart;
        cubeFormationController.OnFormationComplete += HandleFormationComplete;
        cubeFormationController.OnUnwrapStart += HandleUnwrapStart;
    }

    protected virtual void HandleUnwrapComplete()
    {

    }

    protected abstract void HandleUnwrapStart();

    protected abstract void HandleFormationComplete(FormationType formationType);
    protected virtual void HandleFormationStart()
    {

    }

    private void OnDestroy()
    {
        cubeFormationController.OnFormationStart += HandleFormationStart;
        cubeFormationController.OnFormationComplete += HandleFormationComplete;
        cubeFormationController.OnUnwrapStart += HandleUnwrapStart;
    }
}
