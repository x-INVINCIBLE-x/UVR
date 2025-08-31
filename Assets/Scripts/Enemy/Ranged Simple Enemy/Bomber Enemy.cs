using UnityEngine;
using UnityEngine.AI;

public class BomberEnemy : SimpleEnemyBase
{
    public float selfDestructTime = 1f;

    protected override void Update()
    {
        base.Update();

        if (isDead) return;

        if (!playerInSightRange && !playerInAttackRange)
            Patrol();
        else if (playerInSightRange && playerInAttackRange)
            SelfDestruct();
        else if (playerInSightRange)
            Chase();
    }

    private void SelfDestruct()
    {
        FXManager.SelfDestructingVFX(0.5f);
        Destroy(gameObject, selfDestructTime);
    }
}
