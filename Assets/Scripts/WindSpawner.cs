using UnityEngine;
using System.Collections;

public class WindSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject windPrefab; // The VFX prefab
    [SerializeField] private Transform mainCamera;  // Reference to main camera transform

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnInterval = 5f;
    [SerializeField] private float maxSpawnInterval = 10f;
    [SerializeField] private float spawnRadius = 5f;

    [Header("Lifetime Settings")]
    [SerializeField] private float effectLifetime = 3f;

    [Header("Rotation Settings")]
    [Tooltip("Extra rotation offset added to the wind rotation (X, Y, Z).")]
    [SerializeField] private Vector3 rotationOffsetEuler = Vector3.zero;

    private void Start()
    {
        if (mainCamera == null && Camera.main != null)
            mainCamera = Camera.main.transform;

        StartCoroutine(SpawnWindRoutine());
    }

    private IEnumerator SpawnWindRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            SpawnWind();
        }
    }

    private void SpawnWind()
    {
        if (windPrefab == null || mainCamera == null) return;

        // Random position around the camera
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0f; // optional, keeps wind on horizontal plane
        Vector3 spawnPos = mainCamera.position + randomOffset;

        // Rotation to face the camera + optional offset
        Vector3 directionToCamera = mainCamera.position - spawnPos;
        if (directionToCamera != Vector3.zero)
        {
            Quaternion spawnRot = Quaternion.LookRotation(directionToCamera) * Quaternion.Euler(rotationOffsetEuler);
            GameObject wind = Instantiate(windPrefab, spawnPos, spawnRot);
            Destroy(wind, effectLifetime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (mainCamera == null) return;

        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.DrawSphere(mainCamera.position, spawnRadius);
    }
}
