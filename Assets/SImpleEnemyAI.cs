
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform Player;
    public Rigidbody playerRb;

    public GameObject bullet;

    public HomingMissile missile;

    public LayerMask WhatIsground, WhatIsPlayer;

    public GameObject HomingMissile;
    private GameObject currentHomingMissile;


    // Enemy Types
    public enum TypesofEnemies
    {   
        Normal,
        Sniper,
        Laser,
        SelfDestruct,
        Homing
    }

    public TypesofEnemies EnemyTypes;


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
    [SerializeField] public float selfDestructTime;
    private void Awake()
    {
        Player = GameObject.Find("Cube").transform;
        playerRb = GameObject.Find("Cube").GetComponent<Rigidbody>();

        //Player = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        VFXManager = GetComponent<EnemyVFXManager>();
    
    }

    private void Start()
    {
        AssignEnemyValues();
    }
    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, WhatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer);

        switch (EnemyTypes)
        {
            case TypesofEnemies.Normal:
                NormalEnemyBehaviour(); 
                break;
            
            case TypesofEnemies.Sniper:
                SniperEnemyBehavior(); 
                break;

            case TypesofEnemies.Laser:
                LaserEnemyBehaviour();
                break;
        
            case TypesofEnemies.SelfDestruct:
                SelfDestructEnemyBehaviour();
                break;

            case TypesofEnemies.Homing:
                HomingEnemyBehavour();
                break;

        }   



        
    }

    private void AssignEnemyValues()
    {
        if (EnemyTypes == TypesofEnemies.Normal)
        {
            // Logic for normal enemies


        }

        if (EnemyTypes == TypesofEnemies.Sniper)
        {
            // Logic for Sniper Enemies
            sightRange = sightRange * 2f;
            attackRange = attackRange * 2f;

        }

        if (EnemyTypes == TypesofEnemies.Laser)
        {
            // Logic for Laser Enemies
        }

        if(EnemyTypes == TypesofEnemies.SelfDestruct)
        {
            attackRange = attackRange / 4f;
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

    private void SelfDestruct()
    {
        VFXManager.SelfDestructingVFX(1f);
        Destroy(gameObject, selfDestructTime);
    }
    private void NormalEnemyBehaviour()
    {
        if (!playerInSightRange && !playerInAttackRange && goToDestination)
        {
            GoToDestination();
        }

        if (!playerInSightRange && !playerInAttackRange && !goToDestination)
        {
            Patrolling();
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }

        if (playerInSightRange && playerInAttackRange)
        {
            if (!isChargingAttack && !hasAttacked)
            {
                agent.SetDestination(transform.position);
                transform.LookAt(Player);

                StartChargingAttack();
            }

        }
    }

    private void SniperEnemyBehavior()
    {
        if (!playerInSightRange && !playerInAttackRange && goToDestination)
        {
            GoToDestination();
        }

        if (!playerInSightRange && !playerInAttackRange && !goToDestination)
        {
            Patrolling();
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }

        if (playerInSightRange && playerInAttackRange)
        {
            if (!isChargingAttack && !hasAttacked)
            {
                agent.SetDestination(transform.position);
                transform.LookAt(Player);

                StartChargingAttack();
            }

        }
    }

    private void LaserEnemyBehaviour()
    {

    }

    private void SelfDestructEnemyBehaviour()
    {
        if (!playerInSightRange && !playerInAttackRange && goToDestination)
        {
            GoToDestination();
        }

        if (!playerInSightRange && !playerInAttackRange && !goToDestination)
        {
            Patrolling();
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }

        if (playerInSightRange && playerInAttackRange)
        {
            if (!hasAttacked)
            {
                //agent.SetDestination(transform.position);
                //transform.LookAt(Player);
                SelfDestruct();
                //StartChargingAttack();
            }

        }
    }

    private void HomingEnemyBehavour()
    {
        if (!playerInSightRange && !playerInAttackRange && goToDestination)
        {
            GoToDestination();
        }

        if (!playerInSightRange && !playerInAttackRange && !goToDestination)
        {
            Patrolling();
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }

        if (playerInSightRange && playerInAttackRange)
        {
            agent.SetDestination(transform.position);
            transform.LookAt(Player);
            Vector3 spawnOffset = new Vector3(0, 1f, 0);
            Vector3 spawnPosition = transform.position + spawnOffset;

            if (!hasAttacked) { }
            if (currentHomingMissile != null) return;// Prevents duplicates

            currentHomingMissile = Instantiate(HomingMissile, spawnPosition , Quaternion.identity ,this.transform);
            
        }
    }
}
    

