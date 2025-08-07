using System;
using UnityEngine;

public static class GameEvents
{
    public static Action<ObjectiveType> OnElimination;
    public static Action<float> OnGloabalMovementSpeedChange;
    public static Action<float> OnGloablAttackSpeedChange;
    public static event Action<IRewardProvider<GameReward>> OnRewardProvided;

    public static void RaiseReward(IRewardProvider<GameReward> giver)
    {
        OnRewardProvided?.Invoke(giver);
    }
}
