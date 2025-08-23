using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Data", menuName = "Attack Info/Attack Data")]
public class AttackData : ScriptableObject
{
    [HideInInspector] public CharacterStats owner;

    public Stat physicalDamage;
    public Stat ignisDamage;
    public Stat frostDamage;
    public Stat blitzDamage;
    public Stat hexDamage;
    public Stat radianceDamage;
    public Stat gaiaDamage;

    public float burnDamage;
    public float frostSpeedReduction;
    public int blitzSurroundingDamage;
    public float gaiaHealAmount;
}
