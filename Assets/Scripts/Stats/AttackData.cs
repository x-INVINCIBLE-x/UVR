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

    public void Init()
    {
        physicalDamage = new Stat(0);
        ignisDamage = new Stat(0);
        frostDamage = new Stat(0);
        blitzDamage = new Stat(0);
        hexDamage = new Stat(0);
        radianceDamage = new Stat(0);
        gaiaDamage = new Stat(0);

        burnDamage = 0;
        frostSpeedReduction = 0;
        blitzSurroundingDamage = 0;
        gaiaHealAmount = 0;
    }
}
