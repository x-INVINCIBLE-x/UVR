using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public enum Prop { 
    Trees,
    Statues,
    Rocks,
    Turret,
    EnemyMelee,
    EnemyRange,
    JackOGools,
    GoblinShield,
    FellironTurret,
    EmeraldBats,
    Munchers,
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
        public List<Transform> patrolPoints;
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

    private Dictionary<GameObject, List<Transform>> patrolpoints = new();
    private List<GameObject> spawnedEntities = new();
    public bool HasSpawned { get; private set; }  = false;

    public void SpawnEntities()
    {
        if (HasSpawned) return;
        if (PropPool.Instance == null) return;
        if (entities == null || entities.Count == 0) return;
        HasSpawned = true;

        foreach (Entity entity in entities)
        {
            Vector3 worldPosition = transform.TransformPoint(entity.position);
            Quaternion worldRotation = Quaternion.identity;
            if (entity.rotation != Vector3.zero)
                worldRotation = transform.rotation * Quaternion.Euler(entity.rotation);

            GameObject prefabToUse = entity.propPrefab != null
                ? entity.propPrefab
                : PropPool.Instance.GetRandomPrefab(entity.prop, strictlyCurrentDifficulty);

            if (prefabToUse == null) continue;

            GameObject prop = ObjectPool.instance.GetObject(prefabToUse, worldPosition);
            prop.transform.parent = null;

            AssignPatrolPoints(prop, entity);
            // If you also want to apply rotation to the spawned entity:
            prop.transform.rotation = worldRotation;

            prop.transform.parent = transform;
            spawnedEntities.Add(prop);
        }
    }

    public void DespawnEntities()
    {
        if (!HasSpawned) return;
        HasSpawned = false;

        for (int i = 0; i < spawnedEntities.Count; i++)
        {
            if (spawnedEntities[i] != null)
            {
                ObjectPool.instance.ReturnObject(spawnedEntities[i]);
            }
        }

        spawnedEntities.Clear();
    }

    private void AssignPatrolPoints(GameObject prop, Entity entity)
    {
        if (entity.patrolPoints != null && entity.patrolPoints.Count > 0)
        {
            patrolpoints[prop] = entity.patrolPoints;
        }
    }

    public List<Transform> GetPatrolPoints(GameObject entity) => patrolpoints[entity];

    void OnDrawGizmos()
    {
        if (entities == null || entities.Count == 0) return;

        foreach (Entity entity in entities)
        {
            Gizmos.color = colorCode[entity.prop];

            Vector3 position = transform.TransformPoint(entity.position);
            Quaternion rotation = transform.rotation * Quaternion.Euler(entity.rotation);

            Gizmos.DrawSphere(position, 0.5f);

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

    private void OnDrawGizmosSelected()
    {
        if (entities == null || entities.Count == 0) return;

        foreach (Entity entity in entities)
        {
            if (entity.patrolPoints != null && entity.patrolPoints.Count > 0)
            {
                for (int i = 0; i < entity.patrolPoints.Count; i++)
                {
                    Transform patrolPoint = entity.patrolPoints[i];
                    if (patrolPoint == null) continue;

#if UNITY_EDITOR
                    GUIStyle style = new();
                    style.fontSize = 22;
                    style.normal.textColor = colorCode[entity.prop];
                    Vector3 patrolPos = patrolPoint.position;
                    Handles.Label(patrolPos + Vector3.up * 0.25f, (i + 1).ToString(), style);
#endif
                }
            }

        }
    }
}
