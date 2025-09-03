using GLTFast.Schema;
using System.Collections;
using UnityEngine;

public class UmbrellaFormationConsequence : FomationConsequence
{
    [Header("Spawn Settings")]
    //public PhysicsProjectile[] laserBeamsToSpawn;
    public LaserBeam[] laserBeamsToSpawn;
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
    [SerializeField] private float beamWarningDuration = 1f;
    [SerializeField] private float shootingForce;
    [SerializeField] private float laserLifeTime = 10f;
    [SerializeField] private AttackData attackData;

    [SerializeField] private bool isActive = false;
    private Quaternion initialRotation;
    private float timer;

    private Collider spawnerInstance = null;

    protected override void Start()
    {
        base.Start();
        type = FormationType.JapaneseUmbrella;
    }
 
    protected override void HandleUnwrapStart(FormationType formationType)
    {
        if (formationType != type) return;

        if (spawnerInstance)
        {
            Destroy(spawnerInstance.gameObject);
            spawnerInstance = null;
        }
    }

    protected override void HandleFormationComplete(FormationType formationType)
    {
        if (formationType != type) return;

        spawnerInstance = Instantiate(spawnArea, transform.position, spawnArea.transform.rotation);
        
        isActive = true;
        initialRotation = spawnerInstance.transform.localRotation;
        StartCoroutine(StartSpawnRoutine());
        //StartCoroutine(OscillationRoutine());
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

        LaserBeam laserBeamInstance;

        for (int i = 0; i < innerCount; i++)
        {
            GameObject newBeam = ObjectPool.instance.GetObject(GetRandomPrefab().gameObject, GetPointInInnerRadius());
            laserBeamInstance = newBeam.GetComponent<LaserBeam>();
            laserBeamInstance.transform.localRotation = spawnerInstance.transform.localRotation;

            laserBeamInstance.Setup(beamWarningDuration, laserLifeTime, attackData);
            //laserBeamInstance.Init(laserLifeTime, attackData);
            //laserBeamInstance.Launch(spawnerInstance.transform, shootingForce, -spawnerInstance.transform.up);
        }

        for (int i = 0; i < outerCount; i++)
        {
            GameObject newBeam = ObjectPool.instance.GetObject(GetRandomPrefab().gameObject, GetPointInOuterRegion());
            laserBeamInstance = newBeam.GetComponent<LaserBeam>();
            laserBeamInstance.transform.localRotation = spawnerInstance.transform.localRotation;

            laserBeamInstance.Setup(beamWarningDuration, laserLifeTime, attackData);
            //GameObject newProjectile = ObjectPool.instance.GetObject(GetRandomPrefab().gameObject, GetPointInOuterRegion());
            //laserBeamInstance = newProjectile.GetComponent<PhysicsProjectile>();
            //laserBeamInstance.Init(laserLifeTime, attackData);
            //laserBeamInstance.Launch(spawnerInstance.transform, shootingForce, -spawnerInstance.transform.up);
        }
    }

    private LaserBeam GetRandomPrefab()
    {
        if (laserBeamsToSpawn == null || laserBeamsToSpawn.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned to spawn.");
            return null;
        }

        int index = Random.Range(0, laserBeamsToSpawn.Length);
        return laserBeamsToSpawn[index];
    }

    private Vector3 GetPointInInnerRadius()
    {
        Vector3 center = spawnerInstance.bounds.center;
        Vector2 randomCircle = Random.insideUnitCircle * innerRadius;
        
        if (Random.Range(0, 1f) < 0.3f)
        {
            randomCircle = Vector3.zero;
        }
        
        Vector3 spawnPoint = new Vector3(center.x + randomCircle.x, center.y, center.z + randomCircle.y);
        return spawnPoint;
    }

    private Vector3 GetPointInOuterRegion()
    {
        Bounds bounds = spawnerInstance.bounds;
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

    //private IEnumerator OscillationRoutine()
    //{
    //    while (isActive)
    //    {
    //        float rotX = Mathf.PingPong(Time.time * oscillationSpeed * 2, angle * 2) - angle;

    //        spawnerInstance.transform.localRotation = initialRotation * Quaternion.Euler(rotX, 0f, 0f);

    //        yield return null;
    //    }
    //}

    private void OnDrawGizmosSelected()
    {
        if (spawnerInstance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spawnerInstance.bounds.center, spawnerInstance.bounds.size);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnerInstance.bounds.center, innerRadius);
        }
    }
}
