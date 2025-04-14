using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Data", menuName = "Attack Info/Attack Data")]
public class AttackData : ScriptableObject
{
    public Stat physicalDamage;
    public Stat fireDamage;
    public Stat electricalDamage;
}
