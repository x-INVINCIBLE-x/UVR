using System;
using System.Collections.Generic;
using UnityEngine;


public class DungeonMusic : MonoBehaviour
{   
    public AudioClip[] dungeonTheme;
    public AudioClip[] battleTheme;
    private int indexDungeon;
    private int indexBattle;

    [SerializeField] private float transitionTime = 2f;
    private bool inCombat = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomMusic();
        //Debug.Log(dungeonTheme[indexDungeon]);
        //Debug.Log(battleTheme[indexBattle]);

        AudioManager.Instance.PlayMusic(dungeonTheme[indexDungeon]);
        EnemyEventManager.Instance.OnEnemySeePlayer  += OnEngageCombat;
        EnemyEventManager.Instance.OnEnemyLostPlayer += OnDisengageCombat;
        EnemyEventManager.Instance.OnEnemyDeath += OnDisengageCombat; // Also disengage combat when all enemies are dead
    }

    private void OnDestroy()
    {   
        EnemyEventManager.Instance.OnEnemySeePlayer -= OnEngageCombat;
        EnemyEventManager.Instance.OnEnemyLostPlayer -= OnDisengageCombat;
        EnemyEventManager.Instance.OnEnemyDeath -= OnDisengageCombat; // Unsubscribe to avoid memory leaks
    }

    private void OnEngageCombat(int enemyId)
    {
        //RandomMusic();
        //Debug.Log(dungeonTheme[indexDungeon]);
        //Debug.Log(battleTheme[indexBattle]);

        if (!inCombat)
        {
            inCombat = true;
            AudioManager.Instance.PlayMusicWithCrossFade(battleTheme[indexBattle], transitionTime);
        }
    }
    
    private void OnDisengageCombat(int enemyID)
    {
        //RandomMusic();
        //Debug.Log(dungeonTheme[indexDungeon]);
        //Debug.Log(battleTheme[indexBattle]);

        if (inCombat && EnemyEventManager.Instance.ActiveEnemies.Count == 0)
        {
            inCombat = false;
            AudioManager.Instance.PlayMusicWithCrossFade(dungeonTheme[indexDungeon], transitionTime);
        }
    }

    private void RandomMusic() 
    {
        indexDungeon = UnityEngine.Random.Range(0, dungeonTheme.Length);
        indexBattle = UnityEngine.Random.Range(0,battleTheme.Length);
    }

    
}
