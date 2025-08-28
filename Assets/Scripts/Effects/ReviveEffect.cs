using UnityEngine;

[CreateAssetMenu(fileName = "Revive_Effect", menuName = "Effects/Revive")]
public class ReviveEffect : Effect
{
    [SerializeField] private int deathDeficeCount = 1;
    [SerializeField] private uint healthOnRevive = 1;

    override public void Apply()
    {
        base.Apply();

        for (int i = 0; i < deathDeficeCount; i++)
            stats.AddDeathDefice((int)healthOnRevive, this);
    }

    override public void Remove()
    {
        base.Remove();

        stats.RemoveDeathDeficesFromSource(this);
    }
}
