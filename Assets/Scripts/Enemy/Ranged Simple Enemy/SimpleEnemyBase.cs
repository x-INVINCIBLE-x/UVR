using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class SimpleEnemyBase : MonoBehaviour
{ // Base class for all simple enemy types
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform Player;
    [SerializeField] protected EnemyFXHandler FXManager;
    [SerializeField] protected Animator animator;


    [SerializeField] protected float sightRange, attackRange;
    [SerializeField] protected float attackCooldownTime = 2f;
    [SerializeField] protected float magicChargeTime = 2f;
    [SerializeField] protected float walkpointRange;
    [SerializeField] protected float patrollwaitTime;


    [SerializeField] protected Vector3 walkPoint;

    [SerializeField] protected bool searchingWalkPoint = false;
    [SerializeField] protected bool playerInSightRange, playerInAttackRange;
    [SerializeField] protected bool hasAttacked, wasPlayerInSight, isChargingAttack, vfxSpawned;
    [SerializeField] protected bool walkPointSet;// bool to check that the walkpoint vector is set or not

    [SerializeField] protected Vector3 PlayerBodyOffset;

    [SerializeField] protected float lifeTime = 3f;
    [SerializeField] protected AttackData attackData;
    [SerializeField] protected EnemyStats enemyStats;
    [SerializeField] protected MeshDissolver dissolver;


    private Coroutine currentCheckRoutine = null;
    private int enemyID;

    protected virtual void Start()
    {
        Player = PlayerManager.instance.PlayerOrigin.transform;
        animator = GetComponent<Animator>();
        dissolver = GetComponent<MeshDissolver>();
        enemyID = EnemyEventManager.Instance.GetNewEnemyID();
        enemyStats = GetComponent<EnemyStats>();
        enemyStats.OnDeath += HandleDeath;
    }

    private void HandleDeath()
    {

    }

    private void OnEnable()
    {
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
    protected virtual void Update()
    {

    }

    private IEnumerator CheckRoutine()
    {
        while (true)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, LayerMask.GetMask("Player"));
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, LayerMask.GetMask("Player"));
            yield return new WaitForSeconds(0.1f);
        }
    }

    protected virtual void Patrol()
    {
        EnemyEventManager.Instance.LostPlayer(enemyID);

        FXManager.SpawnExclamationMark(false); // turning off exclamation mark

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
            return;
        }

    }

    private void DisableQuestionmark() =>  FXManager.SpawnQuestionMark(false);

    protected virtual void SearchWalkPoint()
    {
        float randomz = Random.Range(-walkpointRange, walkpointRange);
        float randomx = Random.Range(-walkpointRange, walkpointRange);

        walkPoint = new Vector3(transform.position.x + randomx, transform.position.y, transform.position.z + randomz);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(walkPoint, out hit, 5f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }

        searchingWalkPoint = false;
    }

    protected virtual void Chase()
    {
        EnemyEventManager.Instance.SeePlayer(enemyID);

        wasPlayerInSight = true;
        walkPoint = transform.position;
        agent.SetDestination(Player.position);
        FXManager.SpawnQuestionMark(false);
        FXManager.SpawnExclamationMark();// turning on exclamation mark
    }

    protected virtual void Attack()
    {
        EnemyEventManager.Instance.SeePlayer(enemyID);

        wasPlayerInSight = true;
        FXManager.SpawnQuestionMark(false);
        FXManager.SpawnExclamationMark(false); // turning off exclamation mark
    }

    protected virtual void ResetAttack()
    {
        hasAttacked = false;
    }


    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


}
