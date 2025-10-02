using System.Collections.Generic;
using UnityEngine;

public abstract class FomationConsequence : MonoBehaviour
{
    private readonly HashSet<DynamicFormationController> subscribedControllers = new();
    protected FormationType type;

    protected virtual void OnEnable()
    {
        FormationConsequenceManager.OnControllerAdded += SubscribeToController;
        FormationConsequenceManager.OnControllerRemoved += UnsubscribeFromController;
    }

    protected virtual void OnDisable()
    {
        FormationConsequenceManager.OnControllerAdded -= SubscribeToController;
        FormationConsequenceManager.OnControllerRemoved -= UnsubscribeFromController;
    }

    protected virtual void Start()
    {
    }

    private void SubscribeToController(DynamicFormationController controller)
    {
        if (subscribedControllers.Contains(controller)) return;

        controller.OnFormationStart += HandleFormationStart;
        controller.OnFormationComplete += HandleFormationComplete;
        controller.OnUnwrapStart += HandleUnwrapStart;

        ChallengeManager.instance.OnChallengeSuccess += HandleUnwrapComplete;
        ChallengeManager.instance.OnChallengeFail += HandleUnwrapComplete;

        subscribedControllers.Add(controller);
    }

    private void UnsubscribeFromController(DynamicFormationController controller)
    {
        if (!subscribedControllers.Contains(controller)) return;

        controller.OnFormationStart -= HandleFormationStart;
        controller.OnFormationComplete -= HandleFormationComplete;
        controller.OnUnwrapStart -= HandleUnwrapStart;

        ChallengeManager.instance.OnChallengeSuccess -= HandleUnwrapComplete;
        ChallengeManager.instance.OnChallengeFail -= HandleUnwrapComplete;

        subscribedControllers.Remove(controller);
    }

    protected virtual void HandleUnwrapComplete()
    {

    }

    protected abstract void HandleUnwrapStart(FormationType formationType);

    protected abstract void HandleFormationComplete(FormationType formationType);
    protected virtual void HandleFormationStart()
    {

    }
}
