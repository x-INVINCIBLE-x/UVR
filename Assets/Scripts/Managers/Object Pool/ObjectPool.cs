using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private int poolSize = 10;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new ();
    private Dictionary<GameObject, Coroutine> returnRoutines = new();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            Destroy(gameObject);
    }

    public GameObject GetObject(GameObject prefab,Transform target)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Trying to get an object from pool but the prefab is null.");
            return null;
        }

        if (poolDictionary.ContainsKey(prefab) == false)
        {
            InitializeNewPool(prefab);
        }

        if (poolDictionary[prefab].Count == 0)
            CreateNewObject(prefab); // if all objects of this type are in uise, create a new one.

        GameObject objectToGet = poolDictionary[prefab].Dequeue();

        objectToGet.transform.position = target.position;
        objectToGet.transform.parent = null;

        objectToGet.SetActive(true);

        return objectToGet;
    }

    public GameObject GetObject(GameObject prefab, Vector3 target)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Trying to get a null prefab from the object pool.");
            return null;
        }

        if (poolDictionary.ContainsKey(prefab) == false)
        {
            InitializeNewPool(prefab);
        }

        if (poolDictionary[prefab].Count == 0)
            CreateNewObject(prefab); // if all objects of this type are in uise, create a new one.

        GameObject objectToGet = poolDictionary[prefab].Dequeue();

        objectToGet.transform.parent = null;
        objectToGet.transform.position = target;

        objectToGet.SetActive(true);

        return objectToGet;
    }

    public void ReturnObject(GameObject objectToReturn, float delay = .001f)
    {
        if (returnRoutines.TryGetValue(objectToReturn, out Coroutine running))
        {
            StopCoroutine(running);
            returnRoutines.Remove(objectToReturn);
        }

        Coroutine routine = StartCoroutine(DelayReturn(delay, objectToReturn));
        returnRoutines[objectToReturn] = routine;
    }

    private IEnumerator DelayReturn(float delay,GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);

        ReturnToPool(objectToReturn);
    }

    private void ReturnToPool(GameObject objectToReturn)
    {
        if (objectToReturn == null) return;

        if (!objectToReturn.TryGetComponent<PooledObject>(out var pooledObject))
        {
            Destroy(objectToReturn);
            return;
        }

        GameObject originalPrefab = pooledObject.originalPrefab;

        objectToReturn.SetActive(false);
        objectToReturn.transform.parent = transform;
        
        poolDictionary[originalPrefab].Enqueue(objectToReturn);
    }

    private void InitializeNewPool(GameObject prefab)
    {
        poolDictionary[prefab] = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab);
        }
    }

    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform);
        newObject.AddComponent<PooledObject>().originalPrefab = prefab;
        newObject.SetActive(false);

        poolDictionary[prefab].Enqueue(newObject);
    }
}
