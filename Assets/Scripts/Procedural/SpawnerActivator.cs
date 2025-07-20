using System.Collections.Generic;
using UnityEngine;

public class SpawnerActivator : MonoBehaviour
{
    private HashSet<Spawner> activeSpawners;

    private void OnTriggerEnter(Collider other)
    {
        Spawner spawner = other.GetComponentInParent<Spawner>();
        if (spawner != null && !activeSpawners.Contains(spawner))
        {
            spawner.SpawnEntities();
            activeSpawners.Add(spawner);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Spawner spawner = other.GetComponentInParent<Spawner>();
        if (spawner != null && activeSpawners.Contains(spawner))
        {
            spawner.DespawnEntities();
            activeSpawners.Remove(spawner);
        }
    }
}
