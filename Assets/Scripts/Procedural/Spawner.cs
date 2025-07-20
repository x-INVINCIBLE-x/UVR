using System.Collections.Generic;
using UnityEngine;

public enum Prop { 
    Trees,
    Statues,
    Rocks,
    Turret,
    EnemyMelee,
    EnemyRange,
    Extra
}

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public class Entity
    {
        public GameObject propPrefab;
        public Prop prop;
        public Vector3 position;
        public Vector3 rotation;
    }

    private Dictionary<Prop, Color> colorCode = new()
    {
        { Prop.Trees, Color.green },
        { Prop.Statues, Color.white },
        { Prop.Rocks, Color.grey },
        { Prop.Turret, Color.blue },
        { Prop.EnemyMelee, Color.yellow},
        { Prop.EnemyRange, Color.red},
        { Prop.Extra, Color.cyan },
    };

    [SerializeField] private List<Entity> entities;

    [Tooltip("Entities of preceding difficulty will not be spawned")]
    [SerializeField] private bool strictlyCurrentDifficulty;

    private List<GameObject> spawnedEntities = new();

    public void SpawnEntities()
    {
        if (PropPool.Instance == null) return;
        if (entities == null || entities.Count == 0) return;

        foreach (Entity entity in entities)
        {
            Vector3 worldPosition = transform.position + entity.position;
            Quaternion rotation = Quaternion.Euler(entity.rotation);

            GameObject prefabToUse = entity.propPrefab != null
                ? entity.propPrefab
                : PropPool.Instance.GetRandomPrefab(entity.prop, strictlyCurrentDifficulty);

            if (prefabToUse == null) continue;

            GameObject prop = ObjectPool.instance.GetObject(prefabToUse, worldPosition);
            //GameObject prop = Instantiate(prefabToUse, worldPosition, rotation);
            prop.transform.parent = transform;
            spawnedEntities.Add(prop);
        }
    }

    public void DespawnEntities()
    {
        for (int i = 0; i < spawnedEntities.Count; i++)
        {
            if (spawnedEntities[i] != null)
            {
                ObjectPool.instance.ReturnObject(spawnedEntities[i]);
            }
        }

        spawnedEntities.Clear();
    }

    void OnDrawGizmos()
    {
        if (entities == null || entities.Count == 0) return;

        foreach (Entity entity in entities)
        {
            Gizmos.color = colorCode[entity.prop];

            Vector3 position = transform.position + entity.position;
            Gizmos.DrawSphere(position, 0.5f);

            Quaternion rotation = Quaternion.Euler(entity.rotation);
            Vector3 forward = rotation * Vector3.forward;
            Vector3 right = rotation * Vector3.right;
            Vector3 up = rotation * Vector3.up;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, position + forward * 0.2f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, position + right * 0.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(position, position + up * 0.2f);
        }
    }
}
