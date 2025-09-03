using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedTurret : Turret
{
    [Range(1f, 10f)]
    [SerializeField] private float attackSpeedMultiplier = 1.5f;

    private HashSet<ISpeedModifiable> enemies = new HashSet<ISpeedModifiable>();
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
        ISpeedModifiable enemy = other.GetComponentInParent<ISpeedModifiable>();
        if (enemy == null)
            enemy = other.GetComponentInChildren<ISpeedModifiable>();

        if (enemy == null) { return; }
        if (enemies.Contains(enemy)) { return; }

        enemies.Add(enemy);
        enemy.LocalAttackSpeedMultiplier = attackSpeedMultiplier;
    }

    protected override void Deactivate(Collider other)
    {
        ISpeedModifiable enemy = other.GetComponentInParent<ISpeedModifiable>();
        if (enemy == null)
            enemy = other.GetComponentInChildren<ISpeedModifiable>();

        if (enemy == null) { return; }

        if (enemies.Contains(enemy))
        {
            enemy.LocalAttackSpeedMultiplier = 1f;
            enemies.Remove(enemy);
        }
    }
}
