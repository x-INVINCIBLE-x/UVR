
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemyAI : MonoBehaviour
{
    [Header("References")]
    [Space]
    public NavMeshAgent agent;
    public Transform Player;
    public GameObject bullet;
    public LayerMask WhatIsground, WhatIsPlayer;
    public Transform Destination;
    //public Rigidbody playerRb;
    //public HomingMissile missile;

    [Space]
    // Missiles Setup
    public GameObject HomingMissile;
    private GameObject currentHomingMissile;
    [Space]
    // Laser Enemy setup
    public GameObject LaserVFX;
    private GameObject currentLaser;
    private LineRenderer laserRenderer;
    private Coroutine laserFocusRoutine;

    [Space]
    [Header("Conditions")]
    [Space]
   
    public bool goToDestination = false; // Straight to Destination

    bool walkPointSet;// bool to check that the walkpoint vector is set or not

    public bool hasSeenPlayer; // Bool to check if enemy has seen player

    public bool hasAttacked; // Bool to check the player has attacked the player 

    public bool playerInSightRange, playerInAttackRange;// Bool to check if the player is in sight or attack range as specified 

    [Space]
    [Header("Values")]
    [Space]
    public Vector3 walkPoint; // Vector to store walking area transform
    public Vector3 LaserOffset; // Offset for the laser when performing attack on player
    public float walkpointRange; // Defines the range of the walkable area for enemy (Random)
    public float attackCooldownTime; // Time between each attack
    public float sightRange, attackRange;// Range of  attack and Range of sight


    [Space]
    [Header("VFX Settings")]
    [Space]
    public EnemyVFXManager VFXManager;
    public float magicChargeTime = 3f; // The magic circle determines for how long the attack will charge with it

    [SerializeField] private bool isChargingAttack = false;
    [SerializeField] private bool vfxSpawned = false;
    [SerializeField] public float selfDestructTime;


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

    
    private void Awake()
    {
        Player = GameObject.Find("Cube").transform;
        //playerRb = GameObject.Find("Cube").GetComponent<Rigidbody>();
        laserRenderer = LaserVFX.GetComponent<LineRenderer>();
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
            sightRange = sightRange * 1.2f;
            attackRange = attackRange * 2f;
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

        if (!playerInSightRange && !playerInAttackRange && goToDestination)
        {
            GoToDestination();
            DestroyLaser();
        }

        if (!playerInSightRange && !playerInAttackRange && !goToDestination)
        {
            Patrolling();
            DestroyLaser();
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
            DestroyLaser();
        }

        if (playerInSightRange && playerInAttackRange)
        {
            if (!isChargingAttack && !hasAttacked)
            {
                agent.SetDestination(transform.position);
                transform.LookAt(Player);

                PerfromLaserAttack();


            }

        }
        UpdateLaserPositions();

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

    private void PerfromLaserAttack()
    {
        if (currentLaser != null) return; // Prevents duplicates
        Debug.Log("Laser attack is performing");

        currentLaser = Instantiate(LaserVFX, this.transform);
        laserRenderer = currentLaser.GetComponent<LineRenderer>();

       // laserRenderer.SetPosition(0, transform.position);
        //laserRenderer.SetPosition(1, Player.localPosition);

        //if (laserFocusRoutine != null)
        //{
           // StopCoroutine(laserFocusRoutine);
        //}

        //laserFocusRoutine = StartCoroutine(UpdateLaserPositionCoroutine());

    }

    public void DestroyLaser()
    {   // Destroy the laser when not needed
        if(currentLaser!= null)
        {
            Destroy(currentLaser);
            Debug.Log("Laser Destroyed");
        }
        
    }

    private void UpdateLaserPositions()
    {   
        // Updates the laser to track the players position correctly 
        if (currentLaser == null || laserRenderer == null) return;
        laserRenderer.SetPosition(0, transform.position);
        laserRenderer.SetPosition(1, Player.localPosition);

       
    }

    private IEnumerator UpdateLaserPositionCoroutine()
    {
        if (currentLaser != null) yield break;

        if(laserRenderer == null) yield break;


        Vector3 startOffset = Player.position + LaserOffset;
        Vector3 targetPosition = Player.position;

        float laserFocusDuration = 2f;
        float timeElapsed = 0f;

        while(timeElapsed < laserFocusDuration)
        {
            timeElapsed += Time.deltaTime;

            Vector3 interpolatedPos = Vector3.Lerp(startOffset, targetPosition, timeElapsed / laserFocusDuration);

            laserRenderer.SetPosition(0, transform.position);
            laserRenderer.SetPosition(1, interpolatedPos);
        }
    }
}
    

