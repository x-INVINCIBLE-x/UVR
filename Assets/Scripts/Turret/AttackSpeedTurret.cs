using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedTurret : Turret
{
    private HashSet<Enemy> enemies = new HashSet<Enemy>();

    private void Start()
    {
        activationTag = "Enemy";   
    }

    protected override void Activate(Collider other)
    {
        if (!TryGetComponent(out Enemy enemy)) { return; }
        if (enemies.Contains(enemy)) { return; }

        enemies.Add(enemy);

    }

    protected override void Deactivate(Collider other)
    {

    }
}
