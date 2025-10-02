using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpawnerActivator : MonoBehaviour
{
    [SerializeField] private float despawnDelay = 1f;

    // Tracks how many colliders from each spawner are inside the trigger
    private readonly Dictionary<Spawner, int> spawnerColliderCounts = new();

    // Tracks any currently running despawn coroutines for each spawner
    private readonly Dictionary<Spawner, Coroutine> despawnCoroutines = new();

    private void OnTriggerEnter(Collider other)
    {
        Spawner spawner = other.GetComponentInParent<Spawner>(true);
        if (spawner == null)
        {
            if (other.transform.childCount > 0) 
                other.transform.GetChild(0).gameObject.SetActive(true);
            return;
        }

        if (!spawnerColliderCounts.ContainsKey(spawner))
            spawnerColliderCounts[spawner] = 0;

        spawnerColliderCounts[spawner]++;

        // Cancel pending despawn
        if (despawnCoroutines.TryGetValue(spawner, out var runningCoroutine))
        {
            StopCoroutine(runningCoroutine);
            despawnCoroutines.Remove(spawner);
        }

        // Activate and spawn if needed
        if (!spawner.HasSpawned)
        {
            spawner.SpawnEntities();
        }

        spawner.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        Spawner spawner = other.GetComponentInParent<Spawner>();
        if (spawner == null)
        {
            if (other.transform.childCount > 0)
                other.transform.GetChild(0).gameObject.SetActive(false);
            return;
        }
        if (!spawnerColliderCounts.ContainsKey(spawner)) return;

        spawnerColliderCounts[spawner]--;

        if (spawnerColliderCounts[spawner] <= 0)
        {
            despawnCoroutines[spawner] = StartCoroutine(DelayedDespawn(spawner));
        }
    }

    private IEnumerator DelayedDespawn(Spawner spawner)
    {
        yield return new WaitForSeconds(despawnDelay);

        if (spawner != null &&
            spawnerColliderCounts.TryGetValue(spawner, out int count) &&
            count <= 0)
        {
            spawner.DespawnEntities();
            spawner.transform.GetChild(0).gameObject.SetActive(false);
            spawnerColliderCounts.Remove(spawner);
            despawnCoroutines.Remove(spawner);
        }
    }
}
