using UnityEngine;

public static class StatExtensions
{
    public static Stat CombineWith(this Stat a, Stat b, float modifier = 1f)
    {
        Stat combined = new (a.BaseValue + b.BaseValue);

        combined.BaseValue *= modifier;

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

    public static AttackData CombineWith(this CharacterStats a, AttackData b, float modifier = 1f)
    {
        if (a == null || b == null) return null;

        AttackData combined = ScriptableObject.CreateInstance<AttackData>();

        combined.physicalDamage = a.physicalDamage.CombineWith(b.physicalDamage, modifier);
        combined.ignisDamage = a.ignisDamage.CombineWith(b.ignisDamage, modifier);
        combined.frostDamage = a.frostDamage.CombineWith(b.frostDamage, modifier);
        combined.blitzDamage = a.blitzDamage.CombineWith(b.blitzDamage, modifier);
        combined.hexDamage = a.hexDamage.CombineWith(b.hexDamage, modifier);
        combined.radianceDamage = a.radianceDamage.CombineWith(b.radianceDamage, modifier);
        combined.gaiaDamage = a.gaiaDamage.CombineWith(b.gaiaDamage, modifier);

        combined.burnDamage = b.burnDamage;
        combined.frostSpeedReduction = b.frostSpeedReduction;
        combined.blitzSurroundingDamage = b.blitzSurroundingDamage;
        combined.gaiaHealAmount = b.gaiaHealAmount;

        return combined;
    }

    public static AttackData CombineWith(this AttackData a, AttackData b)
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

        combined.burnDamage = a.burnDamage + b.burnDamage;
        combined.frostSpeedReduction = a.frostSpeedReduction + b.frostSpeedReduction;
        combined.blitzSurroundingDamage = a.blitzSurroundingDamage + b.blitzSurroundingDamage;
        combined.gaiaHealAmount = a.gaiaHealAmount + b.gaiaHealAmount;

        return combined;
    }
}
