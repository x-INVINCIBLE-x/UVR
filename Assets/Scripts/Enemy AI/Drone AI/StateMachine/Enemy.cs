using UnityEngine;
using UnityHFSM;
using UnityEngine.AI;

[RequireComponent(typeof(Animator),typeof(NavMeshAgent))]

public class Enemy:MonoBehaviour
{
    [Header("References")]
    private StateMachine<EnemyState,StateEvent> EnemyFSM;
    private Animator Animator;
    private NavMeshAgent Agent;


    [SerializeField] private ShootEnemy ShootPrefab;
    [SerializeField] private Player Player;

    [Header("Sensors")]
    [SerializeField] private PlayerSensor FollowPlayerSensor;
    [SerializeField] private PlayerSensor RangeAttackSensor;

    [Space]
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
        EnemyFSM = new StateMachine<EnemyState, StateEvent>();

        // ADD STATES

        EnemyFSM.AddState(EnemyState.Idle, new IdleState(false, this));
        EnemyFSM.AddState(EnemyState.Chase, new ChaseState(true, this,Player.transform));
        EnemyFSM.AddState(EnemyState.Shoot, new ShootState(true, this,ShootPrefab,onAttack));
        //EnemyFSM.AddState(EnemyState.Search, new SearchState(true, this));
        //EnemyFSM.AddState(EnemyState.Die, new DieState(true, this));

        //EnemyFSM.SetStartState(EnemyState.Idle);

        // ADD Transitions


        // Chase Transitions
        EnemyFSM.AddTriggerTransition(StateEvent.DetectPlayer, new Transition<EnemyState>(EnemyState.Idle , EnemyState.Chase));
        EnemyFSM.AddTriggerTransition(StateEvent.LostPlayer, new Transition<EnemyState>(EnemyState.Chase, EnemyState.Idle));

        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Idle, EnemyState.Chase,
                 (transition) => IsInChasingRange
                                 && Vector3.Distance(Player.transform.position, transform.position) > Agent.stoppingDistance)
             );
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.Idle,
            (transition) => !IsInChasingRange
                            || Vector3.Distance(Player.transform.position, transform.position) <= Agent.stoppingDistance)
            );

        // Shoot Transitions
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.Shoot, ShouldShoot,null, null, true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Idle, EnemyState.Shoot, ShouldShoot, null, null, true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Shoot, EnemyState.Chase, IsNotWithinIdleRange));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Shoot, EnemyState.Idle, IsWithinIdleRange));
        





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
        EnemyFSM.Trigger(StateEvent.LostPlayer);
        IsInChasingRange = false;
    }

    private void FollowPlayerSensor_OnPlayerEnter(Transform Player) 
    {
        EnemyFSM.Trigger(StateEvent.DetectPlayer);
        IsInChasingRange = true;
    }
    private bool ShouldShoot(Transition<EnemyState> Transition) =>
            LastAttackTime + AttackCooldown <= Time.time
                   && IsInShootRange;


    private bool IsWithinIdleRange(Transition<EnemyState> Transition) =>
            Agent.remainingDistance <= Agent.stoppingDistance;

    private bool IsNotWithinIdleRange(Transition<EnemyState> Transition) =>
        !IsWithinIdleRange(Transition);


    private void RangeAttackSensor_OnPlayerExit(Vector3 LastKnownPosition) 
    {
        IsInShootRange = false;
    }

    private void RangeAttackSensor_OnPlayerEnter(Transform Player) 
    {
        IsInShootRange = true;
    }



    private void onAttack(State<EnemyState,StateEvent> State)
    {
        transform.LookAt(Player.transform.position);
        LastAttackTime = Time.time;
    }


    private void Update()
    {
        EnemyFSM.OnLogic();
    }


}
