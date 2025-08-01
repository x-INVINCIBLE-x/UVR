using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FormationGroup
{
    public string groupKey;
    public List<GridFormationData> formations;
}

[CreateAssetMenu(fileName = "GridFormationDatabase", menuName = "GridFormation/GridFormationDatabase")]
public class GridFormationDatabase : ScriptableObject
{
    public List<FormationGroup> formationGroups;

    private readonly Dictionary<string, List<GridFormationData>> groupedFormations = new();
    private readonly Dictionary<string, List<int>> unusedIndicesPerGroup = new();
    private bool isInitialized = false;

    public GridFormationData GetRandomUniqueFormation(string groupKey)
    {
        InitializeIfNeeded();

        if (!groupedFormations.ContainsKey(groupKey))
        {
            Debug.LogWarning($"Group '{groupKey}' not found in database.");
            return null;
        }

        var list = groupedFormations[groupKey];

        if (list == null || list.Count == 0)
        {
            Debug.LogWarning($"Group '{groupKey}' contains no formation data.");
            return null;
        }

        if (!unusedIndicesPerGroup.ContainsKey(groupKey) || unusedIndicesPerGroup[groupKey].Count == 0)
        {
            // refill the list
            unusedIndicesPerGroup[groupKey] = new List<int>();
            for (int i = 0; i < list.Count; i++)
                unusedIndicesPerGroup[groupKey].Add(i);
        }

        var indices = unusedIndicesPerGroup[groupKey];
        int randomIdx = Random.Range(0, indices.Count);
        int chosenIndex = indices[randomIdx];
        indices.RemoveAt(randomIdx);

        return list[chosenIndex];
    }

    public void ResetUsedFormations()
    {
        unusedIndicesPerGroup.Clear();
        foreach (var kvp in groupedFormations)
        {
            var indices = new List<int>();
            for (int i = 0; i < kvp.Value.Count; i++)
                indices.Add(i);
            unusedIndicesPerGroup[kvp.Key] = indices;
        }
    }

    private void InitializeIfNeeded()
    {
        if (isInitialized) return;

        groupedFormations.Clear();

        foreach (var group in formationGroups)
        {
            if (string.IsNullOrEmpty(group.groupKey) || group.formations == null || group.formations.Count == 0)
                continue;

            if (!groupedFormations.ContainsKey(group.groupKey))
                groupedFormations[group.groupKey] = new List<GridFormationData>();

            groupedFormations[group.groupKey].AddRange(group.formations);
        }

        isInitialized = true;
    }
}
