using System;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    protected override void Start()
    {
        base.Start();

        GameEvents.OnRewardProvided += HandleRewardProvided;
    }

    private void HandleRewardProvided(IRewardProvider<GameReward> provider)
    {
        
    }
}
