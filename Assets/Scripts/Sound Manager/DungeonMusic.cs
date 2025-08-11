using System;
using System.Collections.Generic;
using UnityEngine;


public class DungeonMusic : MonoBehaviour
{   
    public AudioClip dungeonTheme;
    public AudioClip battleTheme;

    [SerializeField] private float transitionTime = 2f;
    private bool inCombat = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        AudioManager.Instance.PlayMusic(dungeonTheme);
        EnemyEventManager.Instance.OnEnemySeePlayer  += OnEngageCombat;
        EnemyEventManager.Instance.OnEnemyLostPlayer += OnDisengageCombat;
    }

    private void OnDestroy()
    {
        EnemyEventManager.Instance.OnEnemySeePlayer -= OnEngageCombat;
        EnemyEventManager.Instance.OnEnemyLostPlayer -= OnDisengageCombat;
    }

    private void OnEngageCombat(int enemyId)
    {
        if (!inCombat)
        {
            inCombat = true;
            AudioManager.Instance.PlayMusicWithCrossFade(battleTheme, transitionTime);
        }
    }
    
    private void OnDisengageCombat(int enemyID)
    {
        if(inCombat && EnemyEventManager.Instance.ActiveEnemies.Count == 0)
        {
            inCombat = false;
            AudioManager.Instance.PlayMusicWithCrossFade(dungeonTheme, transitionTime);
        }
    }
}
