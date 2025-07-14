using System;
using UnityEngine;

public static class GameEvents
{
    public static System.Action<ObjectiveType> OnElimination;
    public static System.Action<float> OnGloabalMovementSpeedChange;
    public static System.Action<float> OnGloablAttackSpeedChange;
    public static event Action<IRewardProvider> OnCurrencyGiven;

    public static void RaiseReward(IRewardProvider giver)
    {
        OnCurrencyGiven?.Invoke(giver);
    }
}
