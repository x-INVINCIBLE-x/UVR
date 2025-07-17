using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PropPool : MonoBehaviour
{
    public static PropPool Instance { get; private set; }

    [System.Serializable]
    public class PropEntry
    {
        public Prop propType;
        public int difficultyLevel;
        public List<GameObject> prefabs;
    }

    [SerializeField] private List<PropEntry> propEntries;

    private Dictionary<int ,Dictionary<Prop, List<GameObject>>> pool = new();
    private const int Window_Size = 3;
    private int highestDifficulty = 1;

    // Fills Prop Dictionary from list
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        var possibleProps = 1;
        if (DungeonManager.Instance != null)
            possibleProps = DungeonManager.Instance.DifficultyLevel;
        

        foreach (var entry in propEntries)
        {
            if (!pool.ContainsKey(entry.difficultyLevel))
            {
                pool[entry.difficultyLevel] = new();
                highestDifficulty = Mathf.Max(highestDifficulty, entry.difficultyLevel);
            }

            if (!pool[entry.difficultyLevel].ContainsKey(entry.propType))
                pool[entry.difficultyLevel][entry.propType] = new();

            pool[entry.difficultyLevel][entry.propType].AddRange(entry.prefabs);
        }
    }

    // Pick Props from SLiding Window of 3 : From currentDifficulty - 3 to currentDifficulty
    public GameObject GetRandomPrefab(Prop prop, bool currentDifficultyOnly)
    {
        int currentDifficulty = Mathf.Min(DungeonManager.Instance.DifficultyLevel, highestDifficulty); 

        int difficultyLevel = currentDifficultyOnly ? currentDifficulty
            : Random.Range(Mathf.Max(currentDifficulty - Window_Size, 1), currentDifficulty);

        if (pool[difficultyLevel].TryGetValue(prop, out var list) && list.Count > 0)
        {
            return list[Random.Range(0, list.Count)];
        }
        Debug.LogWarning($"No prefab found for prop type: {prop}");
        return null;
    }
}
