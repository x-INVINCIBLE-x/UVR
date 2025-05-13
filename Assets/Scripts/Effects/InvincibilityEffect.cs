using UnityEngine;

[CreateAssetMenu(fileName = "Invincibility Effect", menuName = "Effects/Invincibility")]
public class InvincibilityEffect : Effect
{
    public override void Execute()
    {
        base.Execute();

        stats.SetInvincibleFor(activeDuration);    
    }
}
