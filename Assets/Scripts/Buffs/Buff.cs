using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum ActivationStatus
{
    AlwaysActive, // -> Anytime 
    OneShot, // -> Once the current level
}

public enum ActivationType
{
    OneOff,
    Timer,
    OnHit,
    OnDamage
}

[CreateAssetMenu(fileName = "New Buff", menuName = "Buffs")]
public class Buff : ScriptableObject
{
    public ActivationType ActivationType;
    public ActivationStatus ActivationStatus;
    public Modifier[] statsToBuff;
    public Effect[] effects;
    public float timer = -10f;
    public float activeDuration = 2f;
    public float cooldownDuration = 2f;
}

[System.Serializable]
public class Modifier
{
    public Stats stat;
    public StatModType modType;
    public float value;
}