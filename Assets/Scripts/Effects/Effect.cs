using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : ScriptableObject
{
    protected CharacterStats stats;
    public bool isActive = true;
    
    public virtual void Apply()
    {
        if (stats == null)
            stats = PlayerManager.instance.player.stats;
    }

    public virtual void Remove()
    {

    }
}