
using UnityEngine;
using UnityEngine.AI;

public class SImpleEnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform Player;

    public GameObject bullet;

    public LayerMask WhatIsground, WhatIsPlayer;


    // Straight to Destination
    public bool goToDestination = false;

    // Patrolling 
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkpointRange;
    public bool hasSeenPlayer;

    public Transform Destination;

    // Attacking 
    public float attackCooldownTime;
    public bool hasAttacked;

    // States

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;



    [Header("VFX Settings")]
    public EnemyVFXManager VFXManager;
    public float magicChargeTime = 3f;

    [SerializeField] private bool isChargingAttack = false;
    [SerializeField] private bool vfxSpawned = false;

    private void Awake()
    {
        //Player = GameObject.Find("Cube").transform;
        Player = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        VFXManager = GetComponent<EnemyVFXManager>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, WhatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer);

        if(!playerInSightRange && !playerInAttackRange && goToDestination)
        {
            GoToDestination();
        }

        if(!playerInSightRange && !playerInAttackRange && !goToDestination)
        {
            Patrolling();
        }

        if(playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }

        if (playerInSightRange && playerInAttackRange)
        {   
            if(!isChargingAttack && !hasAttacked)
            {
                agent.SetDestination(transform.position);
                transform.LookAt(Player);

                StartChargingAttack();
            }
            
        }
    }

    private void Patrolling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }


    private void GoToDestination()
    {
        agent.SetDestination(Destination.position);
    }
    private void SearchWalkPoint()
    {
        float randomz = Random.Range(-walkpointRange, walkpointRange);
        float randomx = Random.Range(-walkpointRange,walkpointRange);

        walkPoint = new Vector3(transform.position.x + randomx,transform.position.y ,transform.position.z + randomz);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, WhatIsground))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(Player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(Player);

        if (!hasAttacked)
        {
            Rigidbody rb = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);

            hasAttacked = true;
            Invoke(nameof(ResetAttack), attackCooldownTime);
        }

       
    }

    private void ResetAttack()
    {
        hasAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // Explode , Slows player , sniper type

    private void StartChargingAttack()
    {
        isChargingAttack = true;

        if (!vfxSpawned)
        {   

            VFXManager.SpawnMagicCircleVFX(magicChargeTime);
            vfxSpawned = true;
        }

        Invoke(nameof(PerformAttack), magicChargeTime);
    }

    private void PerformAttack()
    {
        AttackPlayer();
        isChargingAttack = false;
        vfxSpawned = false; 
        VFXManager.DestroyMagicCircleVFX();

    }
}
