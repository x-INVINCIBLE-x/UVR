using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Speed_Effect", menuName = "Effects/Speed Effect")]
public class SpeedEffect : Effect
{
    [Header("Speed Settings")]
    [Tooltip("Speed Increase in Percentage")]
    [SerializeField] private float speedMultiplierPercent;
    private float SpeedMultiplier => 1 + (speedMultiplierPercent/100);

    public override void Apply()
    {
        base.Apply();

        PlayerManager.instance.ActionMediator.AddSpeedMultiplier(SpeedMultiplier);
    }

    public override void Remove()
    {
        base.Remove();

        PlayerManager.instance.ActionMediator.RemoveSpeedMultiplier(SpeedMultiplier);
    }
}
