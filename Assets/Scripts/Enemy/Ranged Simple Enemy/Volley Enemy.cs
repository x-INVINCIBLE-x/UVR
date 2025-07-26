using MasterStylizedProjectile;
using UnityEngine;
using UnityEngine.AI;

public class VolleyEnemy : SimpleEnemyBase
{
    public GameObject volleyProjectile;
    public Transform projectileSpawnPosition;

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {   
        base.Update();

        if (!playerInSightRange && !playerInAttackRange)
            Patrol();
        else if (playerInAttackRange)
            Attack();
        else if (playerInSightRange)
            Chase();
        
    }

    private void TryVolleyAttack()
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
            Invoke(nameof(VolleyAttack), magicChargeTime);
        }
    }

    private void VolleyAttack()
    {
        GameObject volleySpawn = ObjectPool.instance.GetObject(volleyProjectile.gameObject, projectileSpawnPosition.position + PlayerBodyOffset);
        PhysicsProjectile projectileInstance = volleySpawn.GetComponent<PhysicsProjectile>();

        projectileInstance.Init(lifeTime, attackData);
        LaunchVolley(volleySpawn.transform, Player.position, 2f);

        hasAttacked = true;
        isChargingAttack = false;
        vfxSpawned = false;
        FXManager.DestroyMagicCircleVFX();

        Invoke(nameof(ResetAttack), attackCooldownTime);
    }

    private void LaunchVolley(Transform proj, Vector3 target, float time)
    {
        Vector3 start = proj.position;
        Vector3 toTarget = target - start;
        Vector3 flat = new Vector3(toTarget.x, 0, toTarget.z); // the straight(flat) distance between enemy and player
        float dist = flat.magnitude;

        float vxz = dist / time; // Velocity needed to cover flat distance
        float vy = (toTarget.y - 0.5f * Physics.gravity.y * time * time) / time; // Velocity needed to cover flat distance vertical distance

        Vector3 velocity = flat.normalized * vxz;
        velocity.y = vy;

        proj.GetComponent<Rigidbody>().linearVelocity = velocity;
    }

    protected override void ResetAttack()
    {
        base.ResetAttack();
    }

    protected override void Attack()
    {
        base.Attack();
        TryVolleyAttack();

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
