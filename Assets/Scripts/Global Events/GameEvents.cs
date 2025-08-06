using System;
using UnityEngine;

public static class GameEvents
{
    public static System.Action<ObjectiveType> OnElimination;
    public static System.Action<float> OnGloabalMovementSpeedChange;
    public static System.Action<float> OnGloablAttackSpeedChange;
    public static event Action<IRewardProvider<GameReward>> OnRewardProvided;

    public static void RaiseReward(IRewardProvider<GameReward> giver)
    {
        OnRewardProvided?.Invoke(giver);
    }
}
