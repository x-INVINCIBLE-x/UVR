using UnityEngine;

[CreateAssetMenu(fileName = "Jump_Effect", menuName = "Effects/Jump")]
public class JumpEffect : Effect
{
    public int additionalJumps = 0;

    public override void Apply()
    {
        base.Apply();

        actionMediator.JumpAction.AddJumps(additionalJumps);
    }

    public override void Remove()
    {
        base.Remove();

        actionMediator.JumpAction.RemoveJumps(additionalJumps);
    }
}