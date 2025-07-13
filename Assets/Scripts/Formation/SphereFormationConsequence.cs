using System.Runtime.CompilerServices;
using UnityEngine;

public class SphereFormationConsequence : FomationConsequence
{
    [SerializeField] private FlyingShip ship;
    [SerializeField] private Transform centrePoint;
    [SerializeField] private float radius;
    [SerializeField] private int shipsToSpawn;
    [SerializeField] private float shipLifeTime;
    [SerializeField] private float attackRate;

    protected override void HandleFormationComplete(FormationType formationType)
    {
        if (formationType != type) return;

        float angleStep = 360 / shipsToSpawn;

        for (int i = 0; i < shipsToSpawn; i++)
        {
            float angle = angleStep * i;
            Vector3 position = centrePoint.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, 0, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
            FlyingShip newShip = Instantiate(ship, position, Quaternion.identity);
            newShip.Setup(centrePoint.position, shipLifeTime, attackRate);
        }
    }

    protected override void HandleUnwrapStart(FormationType formationType)
    {
        if (formationType != type) return;
    }
}
