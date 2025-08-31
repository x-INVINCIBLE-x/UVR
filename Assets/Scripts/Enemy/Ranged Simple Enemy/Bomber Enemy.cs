using UnityEngine;
using UnityEngine.AI;

public class BomberEnemy : SimpleEnemyBase
{
    public float selfDestructTime = 0.5f;

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
        FXManager.SelfDestructingVFX(1f);
        Destroy(gameObject, selfDestructTime);
    }
}
