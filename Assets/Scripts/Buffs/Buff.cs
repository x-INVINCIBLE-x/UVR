using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Buffs")]
public class Buff : ScriptableObject
{
    public Modifier[] statsToBuff;
    public Effect[] effects;
    public float timer = -10f;
}

[System.Serializable]
public class Modifier
{
    public Stats stat;
    public StatModType modType;
    public float value;
}