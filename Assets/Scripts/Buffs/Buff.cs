using System.Text;
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
    OnDamage,
    OnHealth
}

[CreateAssetMenu(fileName = "New Buff", menuName = "Buff")]
public class Buff : ScriptableObject
{
    [Header("Details")]
    public string buffName;
    [TextArea]
    public string flavourText;
    public Sprite icon;
    public Color iconColor = Color.white;
    public Material frontMaterial;
    public Material backMaterial;
    public BuffProvider cardDisplay;

    [Header("Activation Property")]
    public ActivationType ActivationType;
    public ActivationStatus ActivationStatus;

    [Header("Modifiers and Effectors")]
    public Modifier[] statsToBuff;
    public Effect[] effects;

    [Header("Activation Timers")]
    public float timer = -10f;
    public float activeDuration = 2f;
    public float cooldownDuration = 2f;
    public bool isStackable = false;
    public int maxStacks;

    public string GetName() => buffName;
    public string GetFlavourText() => flavourText;

    public string GetModifierInfo()
    {
        StringBuilder sb = new();

        foreach (Modifier modifier in statsToBuff)
        {
            string applicator = "";

            applicator = modifier.modType switch
            {
                StatModType.Flat => $"+{modifier.value}",
                StatModType.PercentAdd => $"{modifier.value}%",
                _ => $"x{modifier.value + 1}",
            };
            sb.AppendLine($"{modifier.stat} : {applicator}");
        }

        return sb.ToString();
    }

    public string GetEffectorInfo()
    {
        StringBuilder sb = new();

        foreach (Effect effect in effects)
        {
            sb.AppendLine(effect.GetDescription());
        }

        return sb.ToString();
    }

    public string GetActivationInfo()
    {
        StringBuilder sb = new();

        if (ActivationType == ActivationType.Timer)
            sb.Append("Buff activates after certain time.");
        else if (ActivationType == ActivationType.OnHit)
            sb.Append("Buff activates on taking damage.");
        else if (ActivationType == ActivationType.OnDamage)
            sb.Append("Buff activates on giving damage.");

        string timerText = "immediately.";
        if (timer > 0)
            timerText = $"after {timer} sec on challenge start";

        sb.AppendLine($" Buff comes in effect {timerText}");
        
        if (ActivationStatus == ActivationStatus.AlwaysActive)
        {
            sb.AppendLine("Buff can be used once per level.");
        }
        else
        {
            sb.AppendLine($"Buff reactivates after {cooldownDuration}.");
        }

        return sb.ToString();
    }
}

[System.Serializable]
public class Modifier
{
    public Stats stat;
    public StatModType modType;
    public float value;
}