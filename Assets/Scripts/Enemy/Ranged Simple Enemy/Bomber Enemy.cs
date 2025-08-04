using UnityEngine;
using UnityEngine.AI;

public class BomberEnemy : SimpleEnemyBase
{
    public float selfDestructTime = 1f;

    protected override void Start()
    {
        base.Start(); 
    }

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

    protected override void Patrol()
    {
        base.Patrol();
    }

    protected override void Chase()
    {
        base.Chase();
    }
    protected override void SearchWalkPoint()
    {
        base.SearchWalkPoint();
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
    }
}
