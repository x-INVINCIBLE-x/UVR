using System;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    protected override void Start()
    {
        base.Start();
    }

    public void OnLevelUp(int level)
    {
        Debug.Log("stats levellep up");
        if (difficultyProfile != null)
        {
            difficultyProfile.ApplyModifiers(statDictionary, level, this);
        }
    }
}
