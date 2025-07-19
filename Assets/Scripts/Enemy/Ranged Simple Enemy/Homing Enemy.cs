using UnityEngine;
using UnityEngine.AI;

public class HomingEnemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform Player;
    public GameObject HomingMissile;

    public float sightRange, attackRange;
    private GameObject currentMissile;

    private void Start()
    {
        Player = PlayerManager.instance.PlayerOrigin.transform;
    }
    private void Update()
    {
        bool playerInSight = Physics.CheckSphere(transform.position, sightRange, LayerMask.GetMask("Player"));
        bool playerInAttack = Physics.CheckSphere(transform.position, attackRange, LayerMask.GetMask("Player"));

        if (playerInAttack && playerInSight)
        {
            agent.SetDestination(transform.position);
            transform.LookAt(Player);

            if (currentMissile == null)
            {
                Vector3 spawnOffset = new Vector3(0, 1f, 0);
                currentMissile = Instantiate(HomingMissile, transform.position + spawnOffset, Quaternion.identity, transform);
            }
        }
        else if (playerInSight)
            agent.SetDestination(Player.position);
        else
            agent.SetDestination(transform.position); // Patrol
    }
}
