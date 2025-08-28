using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthStat_Effect", menuName = "Effects/Health Stat")]
public class HealthStatEffect : Effect
{
    [SerializeField] private Stats statToModify;

    [Range(0f, 1f)]
    [SerializeField] private float increaseRate;

    private Stat statToIncrease = null;

    public override void Apply()
    {
        base.Apply();

        stats.OnHealthChanged += HandleHealthChange;

        (float currHealth, float maxHealth) = stats.GetHealth();
        float normalizedHealth = currHealth / maxHealth;

        HandleHealthChange(normalizedHealth);
    }

    private void HandleHealthChange(float normalizedHealth)
    {
        statToIncrease = stats.statDictionary[statToModify];

        statToIncrease.RemoveAllModifiersFromSource(this);

        if (normalizedHealth == 1 || normalizedHealth == 0f)
            return;

        float increaseAmount = increaseRate * (1 - normalizedHealth);
        float baseValue = statToIncrease.BaseValue;
        statToIncrease.AddModifier(new StatModifier(baseValue * increaseAmount, StatModType.Flat, this));
    }

    public override void Remove()
    {
        base.Remove();

        stats.OnHealthChanged -= HandleHealthChange;
    }
}
