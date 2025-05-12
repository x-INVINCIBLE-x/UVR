using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public GameObject[] spawnedObjects;
    public Vector3 boxSize = new Vector3(10, 10, 10);
    public int spawnCount = 10;

    void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnObject();

        }
    }

    void SpawnObject()
    {
        Vector3 spawnPosition = transform.position + new Vector3(
            Random.Range(-boxSize.x / 2, boxSize.x / 2),
            Random.Range(-boxSize.y / 2, boxSize.y / 2),
            Random.Range(-boxSize.z / 2, boxSize.z / 2)
        );

        Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}
