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
        Invoke(nameof(SelfKill),0.1f);
        FXManager.SelfDestructingVFX(1f);
        ObjectPool.instance.ReturnObject(gameObject,selfDestructTime);
    }

    private void SelfKill() => enemyStats.KillCharacter();
}
