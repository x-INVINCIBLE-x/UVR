using UnityEngine;

[CreateAssetMenu(fileName = "Invincibility Effect", menuName = "Effects/Invincibility")]
public class InvincibilityEffect : Effect
{
    public override void Apply()
    {
        base.Apply();

        //stats.SetInvincibleFor(activeDuration);    
    }

    public override void Remove()
    {
        base.Remove();


    }
}
