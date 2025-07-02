using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatScalingData
{
    public Stats stat;
    public float baseValue = 10f;
    public AnimationCurve curve = AnimationCurve.Linear(0, 1, 100, 2);
    public float exponent = 0.2f;
}

[CreateAssetMenu(menuName = "Difficulty/DifficultyProfile_Modifier")]
public class DifficultyProfile : ScriptableObject
{
    public bool useExponentialScaling = false;

    [Header("Per-stat Scaling Settings")]
    public List<StatScalingData> scalingData;

    private Dictionary<Stats, StatScalingData> scalingLookup;

    private void OnEnable()
    {
        scalingLookup = new Dictionary<Stats, StatScalingData>();
        foreach (var data in scalingData)
        {
            scalingLookup[data.stat] = data;
        }
    }

    /// <summary>
    /// Applies difficulty-based modifiers to all stats in a dictionary.
    /// </summary>
    public void ApplyModifiers(Dictionary<Stats, Stat> statDictionary, float difficulty, object source = null)
    {
        EnsureLookupReady();

        foreach (var kvp in statDictionary)
        {
            if (!scalingLookup.TryGetValue(kvp.Key, out var data))
                continue;

            float scale = useExponentialScaling
                ? Mathf.Pow(1 + difficulty, data.exponent)
                : data.curve.Evaluate(difficulty);

            float valueToAdd = data.baseValue * (scale - 1f);
            kvp.Value.AddModifier(new StatModifier(valueToAdd, StatModType.Flat, (int)kvp.Key, source));
        }
    }

    private void EnsureLookupReady()
    {
        if (scalingLookup == null || scalingLookup.Count != scalingData.Count)
        {
            scalingLookup = new Dictionary<Stats, StatScalingData>();
            foreach (var data in scalingData)
            {
                scalingLookup[data.stat] = data;
            }
        }
    }
}
