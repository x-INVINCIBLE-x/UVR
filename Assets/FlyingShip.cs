using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class FlyingShip : MonoBehaviour
{
    private float TimeCounter;

    [Header("Roation Setup")]
    [Space]
    [SerializeField] private float speed;
    private float radius;
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;
    [SerializeField] private float radiusExpansionSpeed; // units per second
    [SerializeField] private float orbitsToReachMaxRadius; // how many full orbits before reaching max

    [Space]
    [Header("Bomb setup")]
    [SerializeField] private PhysicsProjectile projectilePrefab;
    [SerializeField] private AttackData attackData;
    [SerializeField] private Transform[] projectileSpawnPoints;
    [SerializeField] private float projectileForce = 1f;
    private Vector3 previousPosition;
    private float attackCoolDown;
    private float lastAttackTime;

    [Header("Attributes")]
    private float lifeTime;
    [SerializeField] private float projectileLifeTime;

    //private float radiusLerpTime = 0f;
    private bool increasing = true;
    private Vector3 orbitCenter;
    private List<Vector3> pathPoints = new List<Vector3>();
    private float currentMinRadius;
    private float currentMaxRadius;

    void Start()
    {
        increasing = true;
        pathPoints.Add(transform.position);
    }

    public void Setup(Vector3 _orbitCentre, float _lifeTime, float _attackCoolDown)
    {
        orbitCenter = _orbitCentre;
        lifeTime = _lifeTime;
        attackCoolDown = _attackCoolDown;

        radius = (transform.position - orbitCenter).magnitude;
        TimeCounter = Mathf.Atan2(transform.position.z - orbitCenter.z, transform.position.x - orbitCenter.x);
        previousPosition = transform.position;

        // Dynamically set the radius expansion bounds
        currentMinRadius = Mathf.Max(minRadius, radius); // so it never shrinks below spawn radius
        currentMaxRadius = Mathf.Max(maxRadius, radius); // ensures it won't contract immediately

    }

    private void Update()
    {
        DropBombAttack();

        // Orbit timing
        TimeCounter += Time.deltaTime * speed;

        if (TimeCounter > lifeTime)
        {
            // TODO: Neately destroy 
            Destroy(gameObject); 
        }

        // Auto radius speed
        float fullOrbitAngle = Mathf.PI * 2f * orbitsToReachMaxRadius;
        float totalTimeToReach = fullOrbitAngle / speed;
        float autoRadiusSpeed = (maxRadius - minRadius) / totalTimeToReach;
        float actualRadiusSpeed = radiusExpansionSpeed > 0 ? radiusExpansionSpeed : autoRadiusSpeed;

        // Radius expansion
        if (increasing)
        {
            radius += actualRadiusSpeed * Time.deltaTime;
            if (radius >= currentMaxRadius)
            {
                radius = currentMaxRadius;
                increasing = false;
            }
        }
        else
        {
            radius -= actualRadiusSpeed * Time.deltaTime;
            if (radius <= currentMinRadius)
            {
                radius = currentMinRadius;
                increasing = true;
            }
        }


        // Compute new position
        float x = Mathf.Cos(TimeCounter) * radius;
        float z = Mathf.Sin(TimeCounter) * radius;
        float y = orbitCenter.y;

        Vector3 offset = new Vector3(x, 0, z);
        Vector3 newPosition = orbitCenter + offset;

        // Direction = current - previous
        Vector3 direction = newPosition - previousPosition;
        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // Update position and save previous
        transform.position = newPosition;
        previousPosition = newPosition;

        pathPoints.Add(newPosition);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(orbitCenter, 15f); // Center marker

        Gizmos.color = Color.red;
        for (int i = 1; i < pathPoints.Count; i++)
        {
            Gizmos.DrawLine(pathPoints[i - 1], pathPoints[i]);
        }
    }

    public void DropBombAttack()
    {
        if (Time.time - lastAttackTime >= attackCoolDown)
        {
            if (projectilePrefab != null)
            {
                foreach (Transform projectileSpawnPoint in projectileSpawnPoints)
                {
                    Vector3 spawnPosition = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;
                    PhysicsProjectile newProjectile = ObjectPool.instance.GetObject(projectilePrefab.gameObject, spawnPosition).
                        GetComponent<PhysicsProjectile>();

                    newProjectile.Init(projectileLifeTime, attackData);
                    newProjectile.Launch(projectileSpawnPoint, projectileForce, -projectileSpawnPoint.transform.up);

                    lastAttackTime = Time.time;
                }
            }
        }
    }
}
