using UnityEngine;

[CreateAssetMenu(fileName = "Invincibility_Effect", menuName = "Effects/Invincibility")]
public class InvincibilityEffect : Effect
{
    public override void Apply()
    {
        base.Apply();

        stats.SetInvincible(true);    
    }

    public override void Remove()
    {
        base.Remove();

        stats.SetInvincible(false);
    }
}
