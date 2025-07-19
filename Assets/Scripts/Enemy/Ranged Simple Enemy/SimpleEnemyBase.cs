using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class SimpleEnemyBase : MonoBehaviour
{ // Base class for all simple enemy types
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform Player;
    [SerializeField] protected EnemyVFXManager VFXManager;
    [SerializeField] protected Animator animator;

    [SerializeField] protected float sightRange, attackRange;
    [SerializeField] protected float attackCooldownTime = 2f;
    [SerializeField] protected float magicChargeTime = 2f;
    [SerializeField] protected float walkpointRange;
    [SerializeField] protected float patrollwaitTime;

    [SerializeField] protected Vector3 walkPoint;

    [SerializeField] protected bool searchingWalkPoint = false;
    [SerializeField] protected bool playerInSightRange, playerInAttackRange;
    [SerializeField] protected bool hasAttacked, isChargingAttack, vfxSpawned;
    [SerializeField] protected bool walkPointSet;// bool to check that the walkpoint vector is set or not

    [SerializeField] protected Vector3 PlayerBodyOffset;
    protected virtual void Start()
    {
        Player = PlayerManager.instance.PlayerOrigin.transform;
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, LayerMask.GetMask("Player"));
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, LayerMask.GetMask("Player"));
    }

    protected virtual void Patrol()
    {
        if (!walkPointSet && !searchingWalkPoint)
        {
            searchingWalkPoint = true;
            Invoke(nameof(SearchWalkPoint), patrollwaitTime);
            return;
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;
            if (distanceToWalkPoint.magnitude < 1f)
            {
                walkPointSet = false;
            }
        }
    }

    protected virtual void SearchWalkPoint()
    {
        float randomz = Random.Range(-walkpointRange, walkpointRange);
        float randomx = Random.Range(-walkpointRange, walkpointRange);

        walkPoint = new Vector3(transform.position.x + randomx, transform.position.y, transform.position.z + randomz);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(walkPoint, out hit, 3f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }

        searchingWalkPoint = false;
    }

    protected virtual void Chase()
    {
        agent.SetDestination(Player.position);
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
