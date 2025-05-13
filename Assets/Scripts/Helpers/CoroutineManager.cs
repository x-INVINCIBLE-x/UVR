using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager _instance;

    public static CoroutineManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("CoroutineHandler");
                _instance = obj.AddComponent<CoroutineManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    public void StartRoutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
