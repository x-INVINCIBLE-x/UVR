using System.Collections.Generic;
using UnityEngine;

public enum Prop { 
    Trees,
    Statues,
    Rocks,
    Turret,
    Enemy,
    Extra
}

public class FloatingCubes : MonoBehaviour
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
        { Prop.Statues, Color.blue },
        { Prop.Rocks, Color.grey },
        { Prop.Turret, Color.red },
        { Prop.Extra, Color.cyan }
    };

    [SerializeField] private List<Entity> entities;

    private void Start()
    {
        foreach (Entity entity in entities)
        {
            //Vector3 scaledPosition = Vector3.Scale(entity.position, transform.localScale);
            Vector3 worldPosition = transform.position + entity.position;
            Quaternion rotation = Quaternion.Euler(entity.rotation);

            GameObject prefabToUse = entity.propPrefab != null
                ? entity.propPrefab
                : PropPool.Instance.GetRandomPrefab(entity.prop);

            if (prefabToUse == null) continue;

            Collider prefabCollider = prefabToUse.GetComponentInChildren<Collider>();
            if (prefabCollider == null)
            {
                Debug.LogWarning($"No collider found on prefab {prefabToUse.name}");
                continue;
            }

            Vector3 halfExtents = prefabCollider.bounds.extents;
            Vector3 center = worldPosition + prefabCollider.bounds.center - prefabToUse.transform.position;

            Collider[] overlaps = Physics.OverlapBox(center, halfExtents, rotation, ~LayerMask.GetMask("Ignore Raycast"), QueryTriggerInteraction.Ignore);

            bool isBlocked = false;
            string colName =  "";
            foreach (var col in overlaps)
            {
                if (col.transform != transform)
                {
                    colName = col.name;
                    isBlocked = true;
                    break;
                }
            }

            if (isBlocked)
            {
                Debug.Log($"Skipped instantiating {entity.prop} at {transform.name} due to external collision {colName}.");
                continue;
            }

            GameObject prop = Instantiate(prefabToUse, worldPosition, rotation);
            prop.transform.parent = transform;

        }
    }

    void OnDrawGizmos()
    {
        if (entities == null || entities.Count == 0) return;

        foreach (Entity entity in entities)
        {
            Gizmos.color = colorCode[entity.prop];
            Gizmos.DrawSphere(transform.position + entity.position, 0.1f);

            Quaternion rotation = Quaternion.Euler(entity.rotation);
            Vector3 forward = rotation * Vector3.forward;
            Vector3 right = rotation * Vector3.right;
            Vector3 up = rotation * Vector3.up;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(entity.position, entity.position + forward * 0.2f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(entity.position, entity.position + right * 0.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(entity.position, entity.position + up * 0.2f);
        }
    }
}
