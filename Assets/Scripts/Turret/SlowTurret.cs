using UnityEngine;

public class SlowTurret : Turret
{
    [Range(0.1f, 1f)]
    [SerializeField] private float speedMutiplier;

    protected override void Activate()
    {
        PlayerManager.instance.ActionMediator.ModifySpeedMultiplier(speedMutiplier);
    }

    protected override void Deactivate()
    {
        PlayerManager.instance.ActionMediator.ResetMovementSpeed();
    }
}
