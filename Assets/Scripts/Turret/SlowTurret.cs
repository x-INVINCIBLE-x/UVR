using UnityEngine;

public class SlowTurret : Turret
{
    [Range(0.1f, 1f)]
    [SerializeField] private float speedMutiplier;

    protected override void Activate(Collider other)
    {
        PlayerManager.instance.ActionMediator.AddSpeedMultiplier(speedMutiplier);
    }

    protected override void Deactivate(Collider other)
    {
        PlayerManager.instance.ActionMediator.RemoveSpeedMultiplier(speedMutiplier);
    }
}
