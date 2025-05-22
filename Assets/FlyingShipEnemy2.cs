using UnityEngine;
using System.Collections.Generic;

public class FlyingShipEnemy2 : MonoBehaviour
{
    public float TimeCounter;
    [Header("Roation Setup")]
    [Space]
    [SerializeField] private float speed;
    [SerializeField] private float radius;
    [SerializeField] private float maxRadius;
    [SerializeField] private float toReachMaxRadius; // Time in seconds to reach maxRadius

    [Space]
    [Header("Bomb setup")]
    private Vector3 previousPosition;
    [SerializeField] private float attackCoolDown;
    [SerializeField] private float lastAttackTime;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform bombSpawnPoint;
    [SerializeField]

    private float radiusLerpTime = 0f;
    private bool increasing = true;
    private List<Vector3> pathPoints = new List<Vector3>();

    private void Start()
    {
        pathPoints.Add(transform.position);
        previousPosition = transform.position;
    }

    private void Update()
    {   
        DropBombAttack();

        TimeCounter += Time.deltaTime * speed;
        // Update time used for Lerp
        radiusLerpTime += Time.deltaTime;
        float t = Mathf.Clamp01(radiusLerpTime / toReachMaxRadius);

        // Change radius based on increasing or decreasing state
        if (increasing)
        {
            radius = Mathf.Lerp(0, maxRadius, t);
            if (t >= 1f)
            {
                increasing = false;
                radiusLerpTime = 0f;
            }
        }
        else
        {
            radius = Mathf.Lerp(maxRadius, 0, t);
            if (t >= 1f)
            {
                increasing = true;
                radiusLerpTime = 0f;
            }
        }

        float x = Mathf.Cos(TimeCounter) * radius;
        float y = transform.position.y;
        float z = Mathf.Sin(TimeCounter) * radius;

        Vector3 newPosition = new Vector3(x, y, z);
        transform.position = newPosition;
        previousPosition = newPosition; // This is for rotation

        Vector3 direction = newPosition - previousPosition;
        if (direction != Vector3.zero)
        {
           
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        transform.rotation = Quaternion.LookRotation(newPosition);

        pathPoints.Add(newPosition);
    }

    private void OnDrawGizmos()
    {
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
            if (bombPrefab != null)
            {
                Vector3 spawnPosition = bombSpawnPoint != null ? bombSpawnPoint.position : transform.position;
                Instantiate(bombPrefab, spawnPosition, Quaternion.identity);
                lastAttackTime = Time.time;
            }


        }

    }

}
