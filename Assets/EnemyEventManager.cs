using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEventManager : MonoBehaviour
{
    public static EnemyEventManager Instance { get; private set; }

    public event Action<int> OnEnemySeePlayer; // Enemy sees the player
    public event Action<int> OnEnemyLostPlayer; // Enemy lost the player from sight 
    public event Action<int> OnEnemyDeath; // Enemy has died


    public List<int> ActiveEnemies { get; private set;} = new List<int>();
    private int nextEnemyID = 0;

    private void Awake()
    {   
        if(Instance != null && Instance != this)
        {
            Debug.LogWarning(Instance.gameObject.name + " already exists, destroying " + this.gameObject.name);
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

    }

    public int GetNewEnemyID() => nextEnemyID++;

    private void OnEnable()
    {
        ActiveEnemies.Clear();
    }

    private void OnDisable()
    {
        ActiveEnemies.Clear();
    }

    // Call this method to trigger on
    public void SeePlayer(int enemyID)
    {
        if (!ActiveEnemies.Contains(enemyID))
        {
            ActiveEnemies.Add(enemyID);
        }
        OnEnemySeePlayer?.Invoke(enemyID);
    }

    public void LostPlayer(int enemyID)
    {
        if (ActiveEnemies.Contains(enemyID))
        {
            ActiveEnemies.Remove(enemyID);
        }
        OnEnemyLostPlayer?.Invoke(enemyID);
    }

    public void EnemyDeath(int enemyID)
    {
        if (ActiveEnemies.Contains(enemyID))
        {
            ActiveEnemies.Remove(enemyID);
        }
        OnEnemyDeath?.Invoke(enemyID);
    }

}
