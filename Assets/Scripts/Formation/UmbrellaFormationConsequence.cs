using GLTFast.Schema;
using System.Collections;
using UnityEngine;

public class UmbrellaFormationConsequence : FomationConsequence
{
    [Header("Spawn Settings")]
    public PhysicsProjectile[] prefabsToSpawn;
    public float spawnFrequency = 2f;
    public int concurrentSpawnAmount = 5;

    [Header("Spawn Area")]
    public BoxCollider spawnArea;

    [Header("Inner Radius Control")]
    public float innerRadius = 5f;
    [Range(0f, 1f)]
    public float innerRadiusPercentage = 0.5f;

    [Header("Spawner rotation")]
    public float angle = 45f;
    public float oscillationSpeed = 1f;

    [Header("Shooting Info")]
    [SerializeField] private float shootingForce;
    [SerializeField] private float bulletLifeTime = 10f;
    [SerializeField] private AttackData attackData;

    [SerializeField] private bool isActive = false;
    private Quaternion initialRotation;
    private float timer;

    protected override void Start()
    {
        base.Start();
        type = FormationType.JapaneseUmbrella;
    }

 
    protected override void HandleUnwrapStart(FormationType formationType)
    {
        if (formationType != type) return;
    }

    protected override void HandleFormationComplete(FormationType formationType)
    {
        if (formationType != type) return;

        isActive = true;
        initialRotation = spawnArea.transform.localRotation;
        StartCoroutine(StartSpawnRoutine());
        StartCoroutine(OscillationRoutine());
    }

    private IEnumerator StartSpawnRoutine()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(spawnFrequency);
            SpawnObjects();
        }
    }

    private void SpawnObjects()
    {
        int innerCount = Mathf.RoundToInt(concurrentSpawnAmount * innerRadiusPercentage);
        int outerCount = concurrentSpawnAmount - innerCount;

        PhysicsProjectile newPP= null;

        for (int i = 0; i < innerCount; i++)
        {
            GameObject newProjectile = ObjectPool.instance.GetObject(GetRandomPrefab().gameObject, GetPointInInnerRadius());
            newPP = newProjectile.GetComponent<PhysicsProjectile>();
            newPP.Init(bulletLifeTime, attackData);
            newPP.Launch(spawnArea.transform, shootingForce, -spawnArea.transform.up);
        }

        for (int i = 0; i < outerCount; i++)
        {
            GameObject newProjectile = ObjectPool.instance.GetObject(GetRandomPrefab().gameObject, GetPointInOuterRegion());
            newPP = newProjectile.GetComponent<PhysicsProjectile>();
            newPP.Init(bulletLifeTime, attackData);
            newPP.Launch(spawnArea.transform, shootingForce, -spawnArea.transform.up);
        }
    }

    private PhysicsProjectile GetRandomPrefab()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned to spawn.");
            return null;
        }

        int index = Random.Range(0, prefabsToSpawn.Length);
        return prefabsToSpawn[index];
    }

    private Vector3 GetPointInInnerRadius()
    {
        Vector3 center = spawnArea.bounds.center;
        Vector2 randomCircle = Random.insideUnitCircle * innerRadius;
        Vector3 spawnPoint = new Vector3(center.x + randomCircle.x, center.y, center.z + randomCircle.y);
        return spawnPoint;
    }

    private Vector3 GetPointInOuterRegion()
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

    private IEnumerator OscillationRoutine()
    {
        while (isActive)
        {
            float rotX = Mathf.PingPong(Time.time * oscillationSpeed * 2, angle * 2) - angle;

            spawnArea.transform.localRotation = initialRotation * Quaternion.Euler(rotX, 0f, 0f);

            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
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
