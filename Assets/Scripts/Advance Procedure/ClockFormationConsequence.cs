using UnityEngine;

public class ClockFormationConsequence : FomationConsequence
{
    protected override void HandleFormationComplete(FormationType formationType)
    {
        if (formationType != type) return;
    }

    protected override void HandleUnwrapStart()
    {
        
    }
}
