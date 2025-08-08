using UnityEngine;

public class RingFormationConsequence : FomationConsequence
{
    [SerializeField] private float vulnerabilityAmount;

    protected override void HandleFormationComplete(FormationType formationType)
    {
        if (formationType != type) return;

        PlayerManager.instance.Player.Stats.Vulnerability += vulnerabilityAmount;
    }

    protected override void HandleUnwrapStart(FormationType formationType)
    {
        if (formationType != type) return;

        PlayerManager.instance.Player.Stats.Vulnerability -= vulnerabilityAmount;
    }
}
