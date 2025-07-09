using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private float lastTimeUpdatedDistanation;

    public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.SetToRunSpeed();
        enemy.agent.isStopped = false;
        stateTimer = enemy.chaseDuration;

        enemy.visuals.EnableIK(false, true);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.visuals.EnableIK(false, false);
    }

    public override void Update()
    {
        base.Update();

        Debug.Log(enemy.PlayerInAttackRange());
        if (!enemy.IsPlayerHeightRechable() && enemy.IsPlayerReachable())
        {
            stateMachine.ChangeState(enemy.spectingState);
            return;
        }

        if (stateTimer < 0 && !enemy.IsPlayerInAgrresionRange())
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (enemy.PlayerInAttackRange())
            stateMachine.ChangeState(enemy.attackState);

        enemy.FaceTarget(GetNextPathPoint());

        if (CanUpdateDestination())
        {
            enemy.agent.destination = enemy.player.transform.position;
        }
    }


    private bool CanUpdateDestination()
    {
        if (Time.time > lastTimeUpdatedDistanation + .25f)
        {
            lastTimeUpdatedDistanation = Time.time;
            return true;
        }

        return false;
    }
}
