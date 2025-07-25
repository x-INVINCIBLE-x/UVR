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
        Spawner spawner = other.GetComponentInParent<Spawner>();
        if (spawner == null) return;
        spawner.transform.GetChild(0).gameObject.SetActive(true);

        // Cancel pending despawn
        if (despawnCoroutines.TryGetValue(spawner, out var runningCoroutine))
        {
            StopCoroutine(runningCoroutine);
            despawnCoroutines.Remove(spawner);
        }

        // Increase collider count
        if (!spawnerColliderCounts.ContainsKey(spawner))
            spawnerColliderCounts[spawner] = 0;

        spawnerColliderCounts[spawner]++;

        // Spawn entities if not already spawned
        if (!spawner.HasSpawned)
        {
            spawner.SpawnEntities();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Spawner spawner = other.GetComponentInParent<Spawner>();
        
        if (spawner == null || !spawnerColliderCounts.ContainsKey(spawner)) return;

        spawnerColliderCounts[spawner]--;

        if (spawnerColliderCounts[spawner] <= 0)
        {
            // Start delayed despawn
            despawnCoroutines[spawner] = StartCoroutine(DelayedDespawn(spawner));
        }
    }

    private IEnumerator DelayedDespawn(Spawner spawner)
    {
        yield return new WaitForSeconds(despawnDelay);

        // If still no colliders inside, despawn
        if (!spawnerColliderCounts.ContainsKey(spawner) || spawnerColliderCounts[spawner] <= 0)
        {
            spawner.DespawnEntities();
            spawnerColliderCounts.Remove(spawner);
            despawnCoroutines.Remove(spawner);
            spawner.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
