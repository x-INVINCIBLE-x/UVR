using System;
using UnityEngine;

public class CharacterStatsController : MonoBehaviour
{
    [SerializeField] private CharacterStats model;
    [SerializeField] private CharacterStatsView view;

    private void Start()
    {
        if (model.statDictionary == null)
            model.InitializeStatDictionary();

        view.InitializeStats(model.statDictionary);

        model.UpdateHUD += RefreshStats;
        UpdateStats();
        RefreshStats();
    }

    private void UpdateStats()
    {
        foreach (var kvp in model.statDictionary)
        {
            Stats statType = kvp.Key;
            Stat stat = kvp.Value;

            stat.OnValueChanged += (newValue) =>
            {
                if (view.IsVisible)
                    view.UpdateStat(statType, newValue);
            };
        }
    }

    private void RefreshStats()
    {
        view.RefreshAll(model.statDictionary);
    }

    private void OnDestroy()
    {
        model.UpdateHUD -= RefreshStats;
    }
}