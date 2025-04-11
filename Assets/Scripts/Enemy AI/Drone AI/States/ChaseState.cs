using UnityEngine;

public class ChaseState : EnemyStateBase
{
    private Transform Target;

    public ChaseState(bool needsExitTime, Enemy_Drone Enemy , Transform Target) : base(needsExitTime, Enemy)
    {
        this.Target = Target;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Agent.enabled = true;
        Agent.isStopped = false;
        Animator.Play("Weapon_Activate");
    }

    public override void OnLogic()
    {
        base.OnLogic();
        if (!RequestedExit)
        {
            // RequestedExit is true when we try to exit a state 
            // Here we are checking the bool to make sure that the enemy stops when player is out of sight. 
            //If we have not implemented this then the enemy will move towards the last know position of the player
            

            // Important : I want to add it so that the enemy moves towards the player's last known position and add a search state
            Agent.SetDestination(Target.position);
        }
        else if (Agent.remainingDistance <= Agent.stoppingDistance)
        {
            fsm.StateCanExit();
        }
    }
}
