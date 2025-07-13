using UnityEngine;

public class ClockFormationConsequence : FomationConsequence
{
    [SerializeField] private float movementSpeedModifier;
    [SerializeField] private float attackSpeedModifier;

    private void Awake()
    {
        type = FormationType.Clock;    
    }

    protected override void HandleFormationComplete(FormationType formationType)
    {
        if (formationType != type) return;

        GameEvents.OnGloabalMovementSpeedChange?.Invoke(movementSpeedModifier);
        GameEvents.OnGloablAttackSpeedChange?.Invoke(attackSpeedModifier);
    }

    protected override void HandleUnwrapStart(FormationType formationType)
    {
        if (formationType != type) return;

        GameEvents.OnGloabalMovementSpeedChange?.Invoke(1f);
        GameEvents.OnGloablAttackSpeedChange?.Invoke(1f);
    }
}
