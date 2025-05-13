using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : ScriptableObject
{
    protected CharacterStats stats;
    public float activeDuration = 2f;
    public float cooldownDuration = 2f;
    public bool isActive = true;
    
    public virtual void Execute()
    {
        if (stats == null)
            stats = PlayerManager.instance.player.stats;
    }
}