using UnityEngine;

public class BlockTurret : Turret
{
    public GameObject blockerPrefab;

    protected override void Activate()
    {
        blockerPrefab.SetActive(true);
    }

    protected override void Deactivate()
    {
        if (!isSliced) return;

        blockerPrefab.SetActive(false);
    }
}
