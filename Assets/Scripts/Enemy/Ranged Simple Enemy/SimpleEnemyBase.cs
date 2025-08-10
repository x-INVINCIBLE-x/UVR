using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class SimpleEnemyBase : MonoBehaviour, IRewardProvider<GameReward>
{ // Base class for all simple enemy types

    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform Player;
    [SerializeField] protected EnemyFXHandler FXManager;
    [SerializeField] protected Animator animator;
    [SerializeField] private LayerMask allyLayer;
    [SerializeField] private Transform spawnPosition;

    [SerializeField] private GameReward eliminationReward;

    [SerializeField] protected float sightRange, attackRange;
    [SerializeField] protected float attackCooldownTime = 2f;
    [SerializeField] protected float magicChargeTime = 2f;
    [SerializeField] protected float walkpointRange;
    [SerializeField] protected float patrollwaitTime;


    [SerializeField] protected Vector3 walkPoint;

    [SerializeField] protected float speed = 3f;
    [SerializeField] protected bool searchingWalkPoint = false;
    [SerializeField] protected bool playerInSightRange, playerInAttackRange;
    [SerializeField] protected bool hasAttacked, wasPlayerInSight, isChargingAttack, vfxSpawned;
    [SerializeField] protected bool walkPointSet;// bool to check that the walkpoint vector is set or not

    [SerializeField] protected Vector3 PlayerBodyOffset;

    [SerializeField] protected float projectileLifeTime = 3f;
    [SerializeField] protected AttackData attackData;
    [SerializeField] protected EnemyStats enemyStats;
    [SerializeField] protected MeshDissolver dissolver;

    [SerializeField] private float surroundingHitRadius;
    [SerializeField] protected HomingMissile blitzProjectile;
    [SerializeField] protected HomingMissile healProjectile;

    private readonly WaitForSeconds attackCheckCooldown = new(0.2f);
    private LayerMask playerLayer;
    private Coroutine currentCheckRoutine = null;
    private Collider m_Collider;

    private float defaultMagicChargeTime;

    private bool hitSurroundingEnemies = false;
    private bool healPlayer = false;
    private Coroutine healthTimerCoroutine;
    private int enemyID;
    protected bool isDead;
    private bool HealthUIActive = false;
    protected bool attackSoundPlayed = false;

    private AttackData ailmentData;

    private EnemyEventManager enemyEventManager;
 
    private bool registered = false;

    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip idleClip;
    [SerializeField] private AudioClip enemyHitCry;

    protected virtual void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
        Player = PlayerManager.instance.PlayerOrigin.transform;
        animator = GetComponent<Animator>();
        dissolver = GetComponent<MeshDissolver>();
        m_Collider = GetComponent<Collider>();

        enemyEventManager = EnemyEventManager.Instance;

        if (enemyEventManager != null)
            enemyID = enemyEventManager.GetNewEnemyID();

        enemyStats = GetComponent<EnemyStats>();
        enemyStats.OnDamageTaken += HandleHit;
        enemyStats.OnDeath += HandleDeath;
        enemyStats.OnAilmentStatusChange += HandleAilment;
        enemyStats.OnHealthChanged += HandleHealthChange; //Event called when healthchange (gives normalized health value)
        defaultMagicChargeTime = magicChargeTime;
        InitializeAilmentData();
    }

    private void HandleHealthChange(float health)
    {
        FXManager.UpdateHealthValue(health);            
    }

    private void InitializeAilmentData()
    {
        ailmentData = ScriptableObject.CreateInstance<AttackData>();
        ailmentData.physicalDamage = new Stat(0);
        ailmentData.ignisDamage = new Stat(0);
        ailmentData.frostDamage = new Stat(0);
        ailmentData.blitzDamage = new Stat(0);
        ailmentData.hexDamage = new Stat(0);
        ailmentData.radianceDamage = new Stat(0);
        ailmentData.gaiaDamage = new Stat(0);
    }

    private void HandleAilment(AilmentType type, bool isActivated, float effectAmount)
    {
        AilmentStatus status = enemyStats.GetAilmentStatus(type);
        FXManager.SpawnAilmentUI(type,true);
        FXManager.UpdateAilmentValue(isActivated,status);

        // Ailment VFX Spawning
        if (isActivated)
        {
            FXManager.SpawnAilmentVFX(type);
        }
        if (!isActivated)
        {
            FXManager.SpawnAilmentVFX(type, false);
        }


        if (type == AilmentType.Blitz)
        {
            hitSurroundingEnemies = isActivated;
            ailmentData.physicalDamage.BaseValue = effectAmount;
            ailmentData.blitzDamage.BaseValue = effectAmount;
        }
        else if (type == AilmentType.Gaia)
        {
            healPlayer = isActivated;
            ailmentData.physicalDamage.BaseValue = -effectAmount;
            ailmentData.blitzDamage.BaseValue = 0;
        }
        else if (type == AilmentType.Frost)
        {
            agent.speed = isActivated ? speed * 0.5f : speed;
            magicChargeTime = isActivated ? magicChargeTime * 1.5f : defaultMagicChargeTime;
        }
    }   

    protected virtual void HandleHit(float arg1, float arg2)
    {
        if (isDead) return;

        AudioManager.Instance.PlaySFX(enemyHitCry);

        if (hitSurroundingEnemies)
        {
            HashSet<Transform> hitTransforms = new HashSet<Transform>();
            Collider[] colliders = Physics.OverlapSphere(transform.position, surroundingHitRadius, allyLayer);

            foreach (var col in colliders)
            {
                if (hitTransforms.Contains(col.transform.root)) continue;
                if (col.transform.root == transform.root) continue;

                hitTransforms.Add(col.transform.root);
                Debug.Log($"Hit Surrounding Enemy: {col.name}");
                IDamageable damageable = col.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    HomingMissile newMissile = ObjectPool.instance.GetObject(blitzProjectile.gameObject, spawnPosition.position).GetComponent<HomingMissile>();
                    newMissile.Setup(col.GetComponent<Rigidbody>(), ailmentData, 10, 4, projectileLifeTime, 100);
                }
            }
        }

        if (healPlayer)
        {
            Debug.Log("Healing Player");
            HomingMissile newMissile = ObjectPool.instance.GetObject(healProjectile.gameObject, transform.position).GetComponent<HomingMissile>();
            newMissile.Setup(PlayerManager.instance.Rb, ailmentData, 10, 4, projectileLifeTime, 100, playerLayer);
        }

        // Impact Dissolve Effect
        dissolver.StartImpactDissolve(0.1f);

        // Health UI Updation
        if (!HealthUIActive)
        {
            FXManager.SpawnHealthUI(true);
            HealthUIActive = true;
        }
        // Reset the timer if already running
        if (healthTimerCoroutine != null)
            StopCoroutine(healthTimerCoroutine);

        healthTimerCoroutine = StartCoroutine(HealthTimer(3f));

    }

    private IEnumerator HealthTimer(float time)
    {
        yield return new WaitForSeconds(time);
        DespawnHealthUI();
    }
    private void DespawnHealthUI()
    {
        FXManager.SpawnHealthUI(false);
        HealthUIActive = false;
    }

    protected virtual void HandleDeath()
    {
        isDead = true;

        if (agent.enabled)
            agent.SetDestination(transform.position);

        dissolver.StartDissolver();
        if (currentCheckRoutine != null)
        {
            StopCoroutine(currentCheckRoutine);
            currentCheckRoutine = null;
        }

        GameEvents.RaiseReward(this);

        Invoke(nameof(Despawn), 2);
    }

    private void OnEnable()
    {
        isDead = false;
        enemyStats.RestoreStats();
        HideAllDetectionUI();
        wasPlayerInSight = false;
        walkPoint = transform.position;
        currentCheckRoutine = StartCoroutine(CheckRoutine());
    }

    private void OnDisable()
    {
        enemyStats.OnDeath -= HandleDeath;
        if (currentCheckRoutine != null)
        {
            StopCoroutine(currentCheckRoutine);
            currentCheckRoutine = null;
        }
    }

    private void OnDestroy()
    {
        enemyStats.OnDamageTaken -= HandleHit;
        enemyStats.OnDeath -= HandleDeath;
    }

    private void Despawn()
    {
        if (ObjectPool.instance != null)
            ObjectPool.instance.ReturnObject(gameObject);
    }

    protected virtual void Update()
    {
        if (isDead) return;
    }

    private IEnumerator CheckRoutine()
    {
        while (true)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

            yield return attackCheckCooldown;
        }
    }

    protected virtual void Patrol()
    {
        UnregisterEnemy();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;
            distanceToWalkPoint.y = 0;
            if (distanceToWalkPoint.sqrMagnitude < 1f)
            {
                walkPointSet = false;
            }
        }

        if (wasPlayerInSight)
        {
            FXManager.SpawnQuestionMark(); // turn on question mark ui for patrolling
            wasPlayerInSight = false;
            Invoke(nameof(DisableQuestionmark), 4f);
        }

        if (walkPointSet) return;


        if (!walkPointSet && !searchingWalkPoint)
        {
            searchingWalkPoint = true;
            Invoke(nameof(SearchWalkPoint), patrollwaitTime);
            
            AudioManager.Instance.PlaySFX(idleClip);
        }
    }

    private void UnregisterEnemy()
    {
        if (enemyEventManager != null && wasPlayerInSight)
        {
            enemyEventManager.LostPlayer(enemyID);
            FXManager.SpawnExclamationMark(false); // turning off exclamation mark
            registered = false;
        }
    }

    private void DisableQuestionmark() => FXManager.SpawnQuestionMark(false);

    protected virtual void SearchWalkPoint()
    {
        float randomz = Random.Range(-walkpointRange, walkpointRange);
        float randomx = Random.Range(-walkpointRange, walkpointRange);

        walkPoint = new Vector3(transform.position.x + randomx, transform.position.y, transform.position.z + randomz);

        if (NavMesh.SamplePosition(walkPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }

        searchingWalkPoint = false;
    }

    protected virtual void Chase()
    {
        Debug.Log($"Chasing Player: {Player.name}");
        if (enemyEventManager != null)
        {
            enemyEventManager.SeePlayer(enemyID);
        }

        wasPlayerInSight = true;
        walkPoint = transform.position;
        agent.SetDestination(Player.position);
        ShowExclamationOnly();
    }

    protected virtual void Attack()
    {
        RegisterEnemy();

        wasPlayerInSight = true;
        HideAllDetectionUI();

        if (!attackSoundPlayed)
        {
            AudioManager.Instance.PlaySFX(attackClip);
            attackSoundPlayed = true;
        }
    }

    private void RegisterEnemy()
    {
        if (!registered && enemyEventManager != null)
        {
            enemyEventManager.SeePlayer(enemyID);
            registered = true;
        }
    }

    protected virtual void ResetAttack()
    {
        hasAttacked = false;
    }

    private void HideAllDetectionUI()
    {
        FXManager.SpawnQuestionMark(false);
        FXManager.SpawnExclamationMark(false);
    }

    private void ShowExclamationOnly()
    {
        FXManager.SpawnQuestionMark(false);
        FXManager.SpawnExclamationMark(true);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, surroundingHitRadius);
    }

    public GameReward GetReward()
    {
        return eliminationReward;
    }
}
