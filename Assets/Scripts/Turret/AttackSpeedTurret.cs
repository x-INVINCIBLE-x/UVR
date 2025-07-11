using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedTurret : Turret
{
    [Range(1f, 10f)]
    [SerializeField] private float attackSpeedMultiplier = 1.5f;

    private HashSet<Enemy> enemies = new HashSet<Enemy>();
    [SerializeField] private LayerMask enemyLayer;

    protected override void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        isActive = true;
        Activate(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (isActive == false && ((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        isActive = false;
        Deactivate(other);
    }

    protected override void Activate(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null)
            enemy = other.GetComponentInChildren<Enemy>();

        if (enemy == null) { return; }
        if (enemies.Contains(enemy)) { return; }

        enemies.Add(enemy);
        enemy.AttackSpeedMultiplier = attackSpeedMultiplier;
    }

    protected override void Deactivate(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null)
            enemy = other.GetComponentInChildren<Enemy>();

        if (enemy == null) { return; }

        if (enemies.Contains(enemy))
        {
            enemy.AttackSpeedMultiplier = 1f;
            enemies.Remove(enemy);
        }
    }
}
