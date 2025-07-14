using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CharacterStatsView : MonoBehaviour
{
    [SerializeField] private GameObject statRowPrefab;
    [SerializeField] private Transform statContainer;

    public bool IsVisible => gameObject.activeInHierarchy;
    private readonly Dictionary<Stats, TextMeshProUGUI> statValueTexts = new();

    public void InitializeStats(Dictionary<Stats, Stat> stats)
    {
        foreach (var kvp in stats)
        {
            GameObject row = Instantiate(statRowPrefab, statContainer);
            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = kvp.Key.ToString();                     
            texts[1].text = kvp.Value.Value.ToString("F1");         

            statValueTexts[kvp.Key] = texts[1];
        }
    }

    public void UpdateStat(Stats stat, float value)
    {
        if (statValueTexts.ContainsKey(stat))
            statValueTexts[stat].text = value.ToString("F1");
    }

    public void RefreshAll(Dictionary<Stats, Stat> stats)
    {
        foreach (var kvp in stats)
            UpdateStat(kvp.Key, kvp.Value.Value);
    }
}