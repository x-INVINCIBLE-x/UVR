public class DamageResult
{
    public DamageResult(bool _damageGiven, bool _killed, float _finalDamage, CharacterStats _victim)
    {
        damaageGiven = _damageGiven;
        killed = _killed;
        finalDamage = _finalDamage;
        victim = _victim;
    }

    public bool damaageGiven = false;
    public bool killed = false;
    public float finalDamage = 0f;
    public CharacterStats victim = null;
}
