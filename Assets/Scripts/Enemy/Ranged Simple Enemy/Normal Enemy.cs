using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class NormalEnemy : SimpleEnemyBase
{
    public GameObject bullet;
    public float shootForce;
    public Transform projectileSpawn;
    
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
        sfxSource.PlayOneShot(attackClip);

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
}
