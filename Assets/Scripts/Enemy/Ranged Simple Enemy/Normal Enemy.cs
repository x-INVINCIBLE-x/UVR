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

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, LayerMask.GetMask("Player"));
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, LayerMask.GetMask("Player"));

        if (!playerInSightRange && !playerInAttackRange)
            Patrol();
        else if (playerInSightRange && !playerInAttackRange)
            Chase();
        else if (playerInAttackRange)
            TryAttack();
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

    private void TryAttack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(Player.position + PlayerBodyOffset);
        
        if (!isChargingAttack && !hasAttacked)
        {
            isChargingAttack = true;
            if (!vfxSpawned)
            {
                VFXManager.SpawnMagicCircleVFX(magicChargeTime);
                vfxSpawned = true;
            }
            Invoke(nameof(Attack), magicChargeTime);
        }
    }

    private void Attack()
    {
        //Rigidbody rb = Instantiate(bullet, projectileSpawn).GetComponent<Rigidbody>();
        //rb.AddForce(transform.forward * shootForce, ForceMode.Impulse);

        GameObject enemyProjectile = ObjectPool.instance.GetObject(bullet.gameObject, projectileSpawn.position + PlayerBodyOffset);
        PhysicsProjectile projectileInstance = enemyProjectile.GetComponent<PhysicsProjectile>();

        projectileInstance.Init(lifeTime, attackData);
        projectileInstance.Launch(projectileSpawn, shootForce);

        hasAttacked = true;
        isChargingAttack = false;
        vfxSpawned = false;
        VFXManager.DestroyMagicCircleVFX();

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
