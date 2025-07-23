using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IRewardProvider
{
    [SerializeField] private RewardProfile rewardProfile;

    public LayerMask whatIsAlly;
    public LayerMask whatIsPlayer;
    [Space]
    public int healthPoints = 20;
    [Space]
    public float playerYLevel = 6;

    [Header("Idle data")]
    public float idleTime;
    public float aggresionRange;
    public float chaseDuration = 5f;

    [Header("Move data")]
    [SerializeField] protected float walkSpeed = 1.5f;
    [SerializeField] protected float runSpeed = 3;
    public float turnSpeed;
    private bool manualMovement;
    private bool manualRotation;
    private float currentBaseSpeed;

//-------------------------------------------------------------- Movement Speed -----------------------------------------
    [Header("Speed Modifiers")]
    [SerializeField] private float minSpeedMultiplier = 0.1f;
    [SerializeField] private float maxSpeedMultiplier = 3f;

    private float globalSpeedMultiplier = 1f;
    private float localSpeedMultiplier = 1f;

    public float LocalSpeedMultiplier
    {
        get => localSpeedMultiplier;
        set
        {
            if (Mathf.Approximately(localSpeedMultiplier, value)) return;
            localSpeedMultiplier = value;
            UpdateMovementSpeed();
        }
    }
    public float WalkSpeed => walkSpeed * localSpeedMultiplier;
    public float RunSpeed => runSpeed * localSpeedMultiplier;

    [SerializeField] private List<Transform> patrolPoints = new();
    private Vector3[] patrolPointsPosition;
    private int currentPatrolIndex;

    //-------------------------------------------------------------- Attack Speed -----------------------------------------
    [Header("Attack Speed")]
    [SerializeField] private float _cachedAttackSpeedMultiplier = 1f;
    [SerializeField] private float localAttackSpeedMultiplier = 1f;
    [SerializeField] private float globalAttackSpeedMultiplier = 1f;

    private bool isAttackSpeedDirty = true;

    public float AttackSpeedMultiplier
    {
        get
        {
            if (isAttackSpeedDirty)
            {
                _cachedAttackSpeedMultiplier = Mathf.Clamp(localAttackSpeedMultiplier * globalAttackSpeedMultiplier, minSpeedMultiplier, maxSpeedMultiplier);
                isAttackSpeedDirty = false;
            }

            return _cachedAttackSpeedMultiplier;
        }
    }

    public float LocalAttackSpeedMultiplier
    {
        get => localAttackSpeedMultiplier;
        set
        {
            isAttackSpeedDirty = true;
            localAttackSpeedMultiplier = value;
        }
    }
    
    public bool InBattleMode { get; private set; }
    public bool HasPatrolPoints { get; private set; }  = true;
    protected bool isMeleeAttackReady;

    [Header("Refrences")]
    public Transform player {  get; private set; }
    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }
    public Enemy_Visuals visuals { get; private set; }

    public Enemy_Health health { get; private set; }
    public EnemyStats stats { get; private set; }

    public Ragdoll ragdoll { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        stats = GetComponent<EnemyStats>();
        health = GetComponent<Enemy_Health>();
        ragdoll = GetComponent<Ragdoll>();
        visuals = GetComponent<Enemy_Visuals>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        // TODO: Create interface on spawn
        currentPatrolIndex = 0;
    }

    protected virtual void Start()
    {
        stats.OnDeath += Die;
        GameEvents.OnGloabalMovementSpeedChange += HandleGlobalMovementSpeedChange;
        GameEvents.OnGloablAttackSpeedChange += HandleGlobalAttackSpeedChange;

        player = PlayerManager.instance.PlayerOrigin.GetComponent<Transform>();
        currentBaseSpeed = agent.speed;
    }

    protected virtual void Update()
    {
        if (ShouldEnterBattleMode())
            EnterBattleMode();
    }

    protected virtual void InitializePerk()
    {

    }

    protected bool ShouldEnterBattleMode()
    {
        if (IsPlayerInAgrresionRange() && !InBattleMode)
        {
            EnterBattleMode();
            return true;
        }

        return false;
    }

    public virtual void EnterBattleMode() => InBattleMode = true;

    public virtual void ExitBattleMode() => InBattleMode = false;

    public virtual void GetHit(AttackData attackData)
    {
        stats.TakeDamage(attackData);
    }

    public virtual void Die()
    {
        GameEvents.RaiseReward(this);
        visuals.EnableIK(false, false, 1000);
    }

    public void SetToWalkSpeed()
    {
        currentBaseSpeed = walkSpeed;
        UpdateMovementSpeed();
    }

    public void SetToRunSpeed()
    {
        currentBaseSpeed = runSpeed;
        UpdateMovementSpeed();
    }

    public void UpdateMovementSpeed()
    {
        float finalMultiplier = localSpeedMultiplier * globalSpeedMultiplier;

        finalMultiplier = Mathf.Clamp(finalMultiplier, minSpeedMultiplier, maxSpeedMultiplier);

        agent.speed = currentBaseSpeed * finalMultiplier;
    }

    private void HandleGlobalMovementSpeedChange(float newGlobalMultiplier)
    {
        globalSpeedMultiplier = newGlobalMultiplier;
        UpdateMovementSpeed();
    }

    private void HandleGlobalAttackSpeedChange(float newGlobalMultiplier)
    {
        globalSpeedMultiplier = newGlobalMultiplier;
        isAttackSpeedDirty = true;
    }

    public virtual void MeleeAttackCheck(Transform[] damagePoints, float attackCheckRadius,GameObject fx,AttackData damage)
    {
        if (isMeleeAttackReady == false)
            return;

        foreach (Transform attackPoint in damagePoints)
        {
            Collider[] detectedHits =
                Physics.OverlapSphere(attackPoint.position, attackCheckRadius, whatIsPlayer);


            for (int i = 0; i < detectedHits.Length; i++)
            {
                IDamagable damagable = detectedHits[i].GetComponent<IDamagable>();

                if (damagable != null)
                {

                    damagable.TakeDamage(damage);
                    isMeleeAttackReady = false;
                    GameObject newAttackFx = ObjectPool.instance.GetObject(fx, attackPoint);

                    ObjectPool.instance.ReturnObject(newAttackFx, 1);
                    return;
                }
            }

        }

    }

    public void EnableMeleeAttackCheck(bool enable) => isMeleeAttackReady = enable;

    public virtual void BulletImpact( Vector3 force,Vector3 hitPoint,Rigidbody rb)
    {
        if(health.ShouldDie())
            StartCoroutine(DeathImpactCourutine(force,hitPoint,rb));
    }
    private IEnumerator DeathImpactCourutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(.1f);

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public void FaceTarget(Vector3 target,float turnSpeed = 0)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        if (turnSpeed == 0)
            turnSpeed = this.turnSpeed;

        float yRotation = 
            Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }

    #region Animation events
    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;
    public bool ManualMovementActive() => manualMovement;

    public void ActivateManualRotation(bool manualRotation) => this.manualRotation = manualRotation;
    public bool ManualRotationActive() => manualRotation;
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();



    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }

    #endregion

    #region Patrol logic
    public Vector3 GetPatrolDestination()
    {
        if (!HasPatrolPoints) return transform.position;
        if (patrolPointsPosition == null || patrolPointsPosition.Length == 0) 
            InitializePatrolPoints();

        if (patrolPointsPosition == null || patrolPointsPosition.Length == 0) return transform.position;
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];

        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Count)
            currentPatrolIndex = 0;

        return destination;
    }

    private void InitializePatrolPoints()
    {
        patrolPoints.Clear();
        Spawner spawner = transform.root.GetComponentInParent<Spawner>();
        if (spawner != null)
        {
            patrolPoints = spawner.GetPatrolPoints(gameObject);
        }

        patrolPointsPosition = new Vector3[patrolPoints.Count];

        for (int i = 0; i < patrolPoints.Count; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }

        HasPatrolPoints = patrolPointsPosition != null && patrolPointsPosition.Length > 0;
    }

    #endregion

    public bool IsPlayerInAgrresionRange()
    {
        return Vector3.Distance(transform.position, player.position) < aggresionRange;
    }

    public bool IsPlayerReachable() => Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                                        new Vector3(player.position.x, 0, player.position.z)) < aggresionRange;
    public bool IsPlayerHeightRechable() => (Mathf.Abs(transform.position.y - player.position.y) < playerYLevel);

    public (int, int) GetCurrencyReward()
    {
        return rewardProfile ? (rewardProfile.GetGold(), rewardProfile.GetMagika()) : (0, 0);
    }

    private void OnDestroy()
    {
        GameEvents.OnGloabalMovementSpeedChange -= HandleGlobalMovementSpeedChange;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
    }
}
