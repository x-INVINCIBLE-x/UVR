using System.Collections.Generic;
using UnityEngine;

public class PropPool : MonoBehaviour
{
    public static PropPool Instance { get; private set; }

    [System.Serializable]
    public class PropEntry
    {
        public Prop propType;
        public List<GameObject> prefabs;
    }

    [SerializeField] private List<PropEntry> propEntries;

    private Dictionary<Prop, List<GameObject>> pool = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var entry in propEntries)
        {
            if (!pool.ContainsKey(entry.propType))
                pool[entry.propType] = new List<GameObject>();

            pool[entry.propType].AddRange(entry.prefabs);
        }
    }

    public GameObject GetRandomPrefab(Prop prop)
    {
        if (pool.TryGetValue(prop, out var list) && list.Count > 0)
        {
            return list[Random.Range(0, list.Count)];
        }
        Debug.LogWarning($"No prefab found for prop type: {prop}");
        return null;
    }
}
