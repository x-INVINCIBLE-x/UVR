using UnityEngine;

public class UmbrellaFormationConsequence : FomationConsequence
{
    [Header("Spawn Settings")]
    public GameObject prefabToSpawn;
    public float spawnTimer = 2f;
    public int concurrentSpawnAmount = 5;

    [Header("Spawn Area")]
    public BoxCollider spawnArea;

    [Header("Inner Radius Control")]
    public float innerRadius = 5f;
    [Range(0f, 1f)]
    public float innerRadiusPercentage = 0.5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnTimer)
        {
            SpawnObjects();
            timer = 0f;
        }
    }

    protected override void HandleFormationComplete(FormationType formationType)
    {
        if (formationType != type) return;

        Debug.Log("Clock formation");
    }

    protected override void HandleUnwrapStart()
    {

    }

    void SpawnObjects()
    {
        int innerCount = Mathf.RoundToInt(concurrentSpawnAmount * innerRadiusPercentage);
        int outerCount = concurrentSpawnAmount - innerCount;

        for (int i = 0; i < innerCount; i++)
            Instantiate(prefabToSpawn, GetPointInInnerRadius(), Quaternion.identity);

        for (int i = 0; i < outerCount; i++)
            Instantiate(prefabToSpawn, GetPointInOuterRegion(), Quaternion.identity);
    }

    Vector3 GetPointInInnerRadius()
    {
        Vector3 center = spawnArea.bounds.center;
        Vector2 randomCircle = Random.insideUnitCircle * innerRadius;
        Vector3 spawnPoint = new Vector3(center.x + randomCircle.x, center.y, center.z + randomCircle.y);

        spawnPoint.y = spawnArea.bounds.center.y; // lock Y at center
        return spawnPoint;
    }

    Vector3 GetPointInOuterRegion()
    {
        Bounds bounds = spawnArea.bounds;
        Vector3 point;

        int maxAttempts = 10;
        int attempts = 0;

        do
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            point = new Vector3(x, bounds.center.y, z);

            // Check if point is outside inner radius
            float dx = point.x - bounds.center.x;
            float dz = point.z - bounds.center.z;
            float distanceSqr = dx * dx + dz * dz;

            if (distanceSqr > innerRadius * innerRadius)
                break;

            attempts++;
        }
        while (attempts < maxAttempts);

        return point;
    }

    void OnDrawGizmosSelected()
    {
        if (spawnArea)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spawnArea.bounds.center, spawnArea.bounds.size);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnArea.bounds.center, innerRadius);
        }
    }
}
