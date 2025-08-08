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

    public override void RestoreStats()
    {
        base.RestoreStats();

        if (statDictionary != null)
        {
            foreach (Stat stat in statDictionary.Values)
            {
                stat.RemoveAllModifiersFromSource(this);
            }
        }
    }
}
