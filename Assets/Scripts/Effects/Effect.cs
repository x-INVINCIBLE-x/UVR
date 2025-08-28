using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : ScriptableObject
{
    protected CharacterStats stats;
    protected ActionMediator actionMediator;
    [TextArea]
    public string description;
    public bool isActive = true;
    
    public virtual void Apply()
    {
        if (stats == null)
            stats = PlayerManager.instance.Player.Stats;

        if (actionMediator == null)
            actionMediator = PlayerManager.instance.ActionMediator;
    }

    public virtual void Remove()
    {

    }

    public string GetDescription() => description;  
}