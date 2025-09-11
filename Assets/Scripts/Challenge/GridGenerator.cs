using UnityEngine;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour
{
    [System.Serializable]
    public class GridGroup
    {
        public string gridKey;
        public List<GridSetupData> formations;
    }

    public static GridGenerator Instance { get; private set; }

    [SerializeField] public List<GridGroup> formationGroups;

    [SerializeField] private Dictionary<string, List<GridSetupData>> gridSetupDatabase = new();
    private readonly Dictionary<string, List<int>> unusedIndicesPerGroup = new();
    [System.NonSerialized] private bool isInitialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        InitializeIfNeeded();
    }

    private void Start()
    {
        GenerateGrids(ChallengeManager.instance.CurrentChallenge.GetID());
    }

    public void GenerateGrids(string key)
    {
        GridSetupData setupData = GetRandomUniqueFormation(key);

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < setupData.gridFormationControllers.Length; i++)
        {
            if (setupData.gridFormationControllers[i] != null)
            {
                GridFormationController formationController = Instantiate(setupData.gridFormationControllers[i], setupData.positions[i], setupData.rotations[i]);
                formationController.transform.parent = this.transform;
            }
        }
    }

    public GridSetupData GetRandomUniqueFormation(string groupKey)
    {
        InitializeIfNeeded();

        if (!gridSetupDatabase.ContainsKey(groupKey))
        {
            Debug.LogWarning($"Group '{groupKey}' not found in database.");
            return null;
        }

        var list = gridSetupDatabase[groupKey];

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
        foreach (var kvp in gridSetupDatabase)
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

        gridSetupDatabase.Clear();

        foreach (var group in formationGroups)
        {
            if (string.IsNullOrEmpty(group.gridKey) || group.formations == null || group.formations.Count == 0)
                continue;

            if (!gridSetupDatabase.ContainsKey(group.gridKey))
                gridSetupDatabase[group.gridKey] = new List<GridSetupData>();

            gridSetupDatabase[group.gridKey].AddRange(group.formations);
        }

        isInitialized = true;
    }
}
