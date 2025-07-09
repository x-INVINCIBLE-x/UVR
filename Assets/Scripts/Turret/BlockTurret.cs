using UnityEngine;

public class BlockTurret : Turret
{
    public GameObject blockerPrefab;

    protected override void Activate(Collider other)
    {
        blockerPrefab.SetActive(true);
    }

    protected override void Deactivate(Collider other)
    {
        if (!isSliced) return;

        blockerPrefab.SetActive(false);
    }
}
