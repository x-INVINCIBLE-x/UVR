using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class NormalEnemy : SimpleEnemyBase
{
    public GameObject bullet;
    public float shootForce;
    public Transform projectileSpawn;
    
  
    protected override void Start()
    {
        base.Start();   
    }
    protected override void Update()
    {   
        base.Update();

        if (isDead)
            return;

        if (!playerInSightRange && !playerInAttackRange)
            Patrol();
        else if (playerInSightRange && !playerInAttackRange)
            Chase();
        else if (playerInAttackRange)
            Attack();
    }


    protected override void Patrol()
    {
        base.Patrol();    
    }

    protected override void SearchWalkPoint()
    {   
        base.SearchWalkPoint();
    }

    protected override void Chase()
    {
        base.Chase();
    }


    protected override void Attack()
    {
        base.Attack();
        TryAttack();
    }


    private void TryAttack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(Player.position + PlayerBodyOffset);
        
        if (!isChargingAttack && !hasAttacked)
        {
            isChargingAttack = true;
            if (!vfxSpawned)
            {
                FXManager.SpawnMagicCircleVFX(magicChargeTime);
                vfxSpawned = true;
            }
            Invoke(nameof(NormalAttack), magicChargeTime);
        }

        walkPoint = transform.position;
    }

    private void NormalAttack()
    {
       
        GameObject enemyProjectile = ObjectPool.instance.GetObject(bullet.gameObject, projectileSpawn.position + PlayerBodyOffset);
        PhysicsProjectile projectileInstance = enemyProjectile.GetComponent<PhysicsProjectile>();

        projectileInstance.Init(projectileLifeTime, attackData);
        projectileInstance.Launch(projectileSpawn, shootForce);

        hasAttacked = true;
        isChargingAttack = false;
        vfxSpawned = false;
        FXManager.DestroyMagicCircleVFX();

        Invoke(nameof(ResetAttack), attackCooldownTime);
    }

    protected override void ResetAttack()
    {
        base.ResetAttack();
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
    }
}
