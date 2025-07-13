using UnityEngine;

public static class GameEvents
{
    public static System.Action<ObjectiveType> OnElimination;
    public static System.Action<float> OnGloabalMovementSpeedChange;
    public static System.Action<float> OnGloablAttackSpeedChange;
}
