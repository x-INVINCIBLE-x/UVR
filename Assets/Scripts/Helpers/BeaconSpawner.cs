using UnityEngine;

public class BeaconSpawner : MonoBehaviour
{
    [SerializeField] private GameObject beaconPrefab;
    [SerializeField] private Transform spawnPoint;
    private void Start()
    {
        SpawnBeacon();
    }
    private void SpawnBeacon()
    {
        if (beaconPrefab != null && spawnPoint != null)
        {
            Instantiate(beaconPrefab, spawnPoint);
        }
        else
        {
            Debug.LogWarning("Beacon Prefab or Spawn Point is not assigned.");
        }
    }
}
