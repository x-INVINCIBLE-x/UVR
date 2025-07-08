using UnityEngine;

public static class StatExtensions
{
    public static Stat CombineWith(this Stat a, Stat b)
    {
        Stat combined = new (a.BaseValue + b.BaseValue);

        foreach (var mod in a.StatModifiers)
            combined.AddModifier(mod.Clone());

        foreach (var mod in b.StatModifiers)
            combined.AddModifier(mod.Clone());

        return combined;
    }

    public static StatModifier Clone(this StatModifier mod)
    {
        return new StatModifier(mod.Value, mod.Type, mod.Order, mod.Source);
    }

    public static AttackData CombineWith(this CharacterStats a, AttackData b)
    {
        if (a == null || b == null) return null;

        AttackData combined = ScriptableObject.CreateInstance<AttackData>();

        combined.physicalDamage = a.physicalDamage.CombineWith(b.physicalDamage);
        combined.ignisDamage = a.ignisDamage.CombineWith(b.ignisDamage);
        combined.frostDamage = a.frostDamage.CombineWith(b.frostDamage);
        combined.blitzDamage = a.blitzDamage.CombineWith(b.blitzDamage);
        combined.hexDamage = a.hexDamage.CombineWith(b.hexDamage);
        combined.radianceDamage = a.radianceDamage.CombineWith(b.radianceDamage);
        combined.gaiaDamage = a.gaiaDamage.CombineWith(b.gaiaDamage);

        return combined;
    }
}
