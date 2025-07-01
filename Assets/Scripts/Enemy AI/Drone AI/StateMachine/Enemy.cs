using UnityEngine;
using UnityHFSM;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class Enemy_Drone : MonoBehaviour
{
    [Header("References")]
    private StateMachine<EnemyDrone_State, StateEvent> EnemyFSM;
    private Animator Animator;
    private NavMeshAgent Agent;
    [SerializeField] private Transform Model;  // Drag your 3D model GameObject here

    [SerializeField] private ShootEnemy ShootPrefab;
    [SerializeField] private Player Player;

    [Header("Sensors")]
    [SerializeField] private PlayerSensor FollowPlayerSensor;
    [SerializeField] private PlayerSensor RangeAttackSensor;

    [Header("Debug")]
    [SerializeField] private bool IsInChasingRange;
    [SerializeField] private bool IsInShootRange;
    [SerializeField] private float LastAttackTime;

    [Header("Attack Config")]
    [SerializeField]
    [Range(0.1f, 5f)]
    private float AttackCooldown = 2;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        EnemyFSM = new StateMachine<EnemyDrone_State, StateEvent>();


        // Prevent NavMeshAgent from controlling rotation
        Agent.updateRotation = false;



        // ADD STATES
        EnemyFSM.AddState(EnemyDrone_State.Idle, new IdleState(false, this));
        EnemyFSM.AddState(EnemyDrone_State.Chase, new ChaseState(true, this, Player.transform));
        EnemyFSM.AddState(EnemyDrone_State.Shoot, new ShootState(true, this, ShootPrefab, onAttack));

        // SET START STATE
        EnemyFSM.SetStartState(EnemyDrone_State.Idle);

        // ADD Transitions
        EnemyFSM.AddTriggerTransition(StateEvent.DetectPlayer, new Transition<EnemyDrone_State>(EnemyDrone_State.Idle, EnemyDrone_State.Chase));
        EnemyFSM.AddTriggerTransition(StateEvent.LostPlayer, new Transition<EnemyDrone_State>(EnemyDrone_State.Chase, EnemyDrone_State.Idle));

        // Chase Logic
        EnemyFSM.AddTransition(new Transition<EnemyDrone_State>(
            EnemyDrone_State.Idle, EnemyDrone_State.Chase,
            (transition) => IsInChasingRange && Vector3.Distance(Player.transform.position, transform.position) > Agent.stoppingDistance
        ));

        EnemyFSM.AddTransition(new Transition<EnemyDrone_State>(
            EnemyDrone_State.Chase, EnemyDrone_State.Idle,
            (transition) => !IsInChasingRange || Vector3.Distance(Player.transform.position, transform.position) <= Agent.stoppingDistance
        ));

        // Shoot Transitions (Fixed)
        EnemyFSM.AddTransition(new Transition<EnemyDrone_State>(
            EnemyDrone_State.Chase, EnemyDrone_State.Shoot, ShouldShoot
        ));

        EnemyFSM.AddTransition(new Transition<EnemyDrone_State>(
            EnemyDrone_State.Shoot, EnemyDrone_State.Chase, IsNotWithinIdleRange
        ));

        EnemyFSM.AddTransition(new Transition<EnemyDrone_State>(
            EnemyDrone_State.Shoot, EnemyDrone_State.Idle, IsWithinIdleRange
        ));

        EnemyFSM.Init();
    }

    private void Start()
    {
        FollowPlayerSensor.OnPlayerEnter += FollowPlayerSensor_OnPlayerEnter;
        FollowPlayerSensor.OnPlayerExit += FollowPlayerSensor_OnPlayerExit;
        RangeAttackSensor.OnPlayerEnter += RangeAttackSensor_OnPlayerEnter;
        RangeAttackSensor.OnPlayerExit += RangeAttackSensor_OnPlayerExit;
    }

    private void FollowPlayerSensor_OnPlayerExit(Vector3 LastKnownPosition)
    {
        Debug.Log("Player lost, returning to Idle.");
        EnemyFSM.Trigger(StateEvent.LostPlayer);
        IsInChasingRange = false;
    }

    private void FollowPlayerSensor_OnPlayerEnter(Transform Player)
    {
        Debug.Log("Player detected, chasing.");
        EnemyFSM.Trigger(StateEvent.DetectPlayer);
        IsInChasingRange = true;
    }

    private bool ShouldShoot(Transition<EnemyDrone_State> Transition)
    {
        Debug.Log("Checking if should shoot...");
        return (LastAttackTime + AttackCooldown <= Time.time) && IsInShootRange;
    }

    private bool IsWithinIdleRange(Transition<EnemyDrone_State> Transition)
    {
        return Agent.remainingDistance <= Agent.stoppingDistance;
    }

    private bool IsNotWithinIdleRange(Transition<EnemyDrone_State> Transition)
    {
        return !IsWithinIdleRange(Transition);
    }

    private void RangeAttackSensor_OnPlayerExit(Vector3 LastKnownPosition)
    {
        Debug.Log("Player out of shoot range.");
        IsInShootRange = false;
    }

    private void RangeAttackSensor_OnPlayerEnter(Transform Player)
    {
        Debug.Log("Player within shoot range.");
        IsInShootRange = true;
    }

    private void onAttack(State<EnemyDrone_State, StateEvent> State)
    {
        Debug.Log("Enemy attacking!");

        // Direction to the Core
        Vector3 directionToPlayer = (Player.transform.position - transform.position).normalized;

        // Calculate rotation using LookRotation
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        // Apply rotation smoothly
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        LastAttackTime = Time.time;
    }
    private void RotateTowardsPlayer()
    {
        if (!Player) return; // Safety check

        Vector3 directionToPlayer = Player.transform.position - Model.position;

        // Keep Y-axis rotation but allow looking up/down
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        // Smooth rotation
        Model.rotation = Quaternion.Slerp(Model.rotation, targetRotation, Time.deltaTime * 5f);
    }



    private void Update()
    {
        EnemyFSM.OnLogic();

        if (IsInChasingRange || IsInShootRange)
        {
            RotateTowardsPlayer();  // Rotate only the Model
        }
    }
}
