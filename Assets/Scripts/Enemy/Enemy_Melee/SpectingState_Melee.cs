using UnityEngine;

public class SpectingState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private int spectingTimer = 4;

    public SpectingState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.visuals.EnableIK(false, true);
        stateTimer = spectingTimer;

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
        enemy.agent.SetDestination(enemy.transform.position);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.visuals.EnableIK(false, false);
        enemy.agent.isStopped = false;
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.player.position);

        if (enemy.IsPlayerReachable() && enemy.IsPlayerHeightRechable())
        {
            stateMachine.ChangeState(enemy.attackState);
        }
        else if (!enemy.IsPlayerReachable() && !enemy.IsPlayerHeightRechable())
        {
            stateMachine.ChangeState(enemy.idleState);
        }
        else if (!enemy.IsPlayerInAgrresionRange())
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
