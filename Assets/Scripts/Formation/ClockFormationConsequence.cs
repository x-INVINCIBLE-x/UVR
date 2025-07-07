using UnityEngine;

public class ClockFormationConsequence : FomationConsequence
{
    private void Awake()
    {
        type = FormationType.Clock;    
    }

    protected override void HandleFormationComplete(FormationType formationType)
    {
        if (formationType != type) return;

        Debug.Log("Clock formation");
    }

    protected override void HandleUnwrapStart()
    {
        
    }
}
