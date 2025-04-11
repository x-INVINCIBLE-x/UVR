using UnityEngine;
using UnityHFSM;
using System;
using UnityEngine.AI;
using Unity.VisualScripting;

public abstract class EnemyStateBase : State<EnemyDrone_State,StateEvent>
{
    protected readonly Enemy_Drone Enemy;
    protected readonly NavMeshAgent Agent;
    protected readonly Animator Animator;
    protected bool RequestedExit;
    protected float ExitTime;

    
    protected readonly Action<State<EnemyDrone_State, StateEvent>> onEnter;
    protected readonly Action<State<EnemyDrone_State, StateEvent>> onLogic;
    protected readonly Action<State<EnemyDrone_State, StateEvent>> onExit;
    protected readonly Func<State<EnemyDrone_State, StateEvent>, bool> canExit;

    public EnemyStateBase(bool needsExitTime, 
        Enemy_Drone Enemy,
        float ExitTime = 0.1f,
        Action<State<EnemyDrone_State, StateEvent>> onEnter = null,
        Action<State<EnemyDrone_State, StateEvent>> onLogic = null,
        Action<State<EnemyDrone_State, StateEvent>> onExit = null,
        Func<State<EnemyDrone_State, StateEvent>, bool> canExit = null)
    {
        this.Enemy = Enemy;
        this.onEnter = onEnter;
        this.onLogic = onLogic;
        this.onExit = onExit;
        this.canExit = canExit;
        this.ExitTime = ExitTime;
        this.needsExitTime = needsExitTime;
        Agent = Enemy.GetComponent<NavMeshAgent>();
        Animator = Enemy.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        RequestedExit = false;
        onEnter?.Invoke(this);
    }

    public override void OnLogic()
    {
        base.OnLogic();
        if(RequestedExit && timer.Elapsed >= ExitTime)
        {
            fsm.StateCanExit();
        }
    }

    public override void OnExitRequest()
    {
        if(!needsExitTime || canExit != null && canExit(this))
        {
            fsm.StateCanExit();
        }
        else
        {
            RequestedExit = true;   
        }
    }

}
