using System;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    protected override void Start()
    {
        base.Start();

        if (difficultyProfile != null && DungeonManager.Instance != null)
        {
            difficultyProfile.ApplyModifiers(statDictionary, DungeonManager.Instance.DifficultyLevel, this);
        }
    }
}
