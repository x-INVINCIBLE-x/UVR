using UnityEngine;

public class IdleState : EnemyStateBase
{
    public IdleState(bool needsExitTime , Enemy_Drone Enemy) : base(needsExitTime , Enemy) 
    {
        
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Agent.isStopped = true;
        Animator.Play("Idle");
    }

}
