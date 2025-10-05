using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;
    public GameObject playerPrefab;

    [Header("Settings")]
    public bool friendlyFire;

    public int highestDepthReached = 0;
    public int lastDepth = 0;

    public int totalEnemiesKilled = 0;
    public int totalGoolsKilled = 0;
    public int totalShieldKilled = 0;
    public int totalMuncherKilled = 0;
    public int totalBatzKilled = 0;
    public int totalIronturetKilled = 0;
    public int totalTurretsDestroyed = 0;

    public int enemiesEliminatedLastRound = 0;
    public int goolsKilledLastRound = 0;
    public int shieldKilledLastRound = 0;
    public int muncherKilledLastRound = 0;
    public int batzKilledLastRound = 0;
    public int ironturetKilledLastRound = 0;
    public int turretsDestroyedLastRound = 0;

    public int totalAttempts = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        GameObject Core = Instantiate(playerPrefab, transform.position, transform.rotation);
        Core.name = "Core";
    }

    public void SetLastRoundData(ObjectiveType type, int amount)
    {
        switch (type)
        {
            case ObjectiveType.JackOGools:
                goolsKilledLastRound = amount;
                totalGoolsKilled += amount;
                break;
            case ObjectiveType.GoblinShield:
                shieldKilledLastRound = amount;
                totalShieldKilled += amount;
                break;
            case ObjectiveType.FellironTurret:
                ironturetKilledLastRound = amount;
                totalIronturetKilled += amount;
                break;
            case ObjectiveType.EmeraldBats:
                batzKilledLastRound = amount;
                totalBatzKilled += amount;
                break;
            case ObjectiveType.Turret:
                turretsDestroyedLastRound = amount;
                totalTurretsDestroyed += amount;
                break;
        }

        enemiesEliminatedLastRound += amount;
        totalAttempts++;
    }

    public void SetLastLevel(int level)
    {
        lastDepth = level;
        highestDepthReached = Mathf.Max(highestDepthReached, level);
        totalEnemiesKilled += enemiesEliminatedLastRound;

        if (SavingWrapper.instance != null)
            SavingWrapper.instance.Save();
    }

    public object CaptureState()
    {
        TempAchievements temp = new()
        {
            highestDepthReached = highestDepthReached,
            lastSDepth = lastDepth,

            totalEnemiesKilled = totalEnemiesKilled,
            totalGoolsKilled = totalGoolsKilled,
            totalShieldKilled = totalShieldKilled,
            totalBatzKilled = totalBatzKilled,
            totalMuncherKilled = totalMuncherKilled,
            totalIronturetKilled = totalIronturetKilled,
            totalTurretsDestroyed = totalTurretsDestroyed,

            enemiesEliminatedLastRound = enemiesEliminatedLastRound,
            goolsKilledLastRound = goolsKilledLastRound,
            shieldKilledLastRound = shieldKilledLastRound,
            muncherKilledLastRound = muncherKilledLastRound,
            batzKilledLastRound = batzKilledLastRound,
            ironturetKilledLastRound = ironturetKilledLastRound,
            turretsDestroyedLastRound = turretsDestroyedLastRound,

            totalAttempts = totalAttempts
        };

        return temp;
    }

    public void RestoreState(object state)
    {
        TempAchievements temp = (TempAchievements)state;

        highestDepthReached = temp.highestDepthReached;
        lastDepth = temp.lastSDepth;

        totalEnemiesKilled = temp.totalEnemiesKilled;
        totalGoolsKilled = temp.totalGoolsKilled;
        totalShieldKilled = temp.totalShieldKilled;
        totalMuncherKilled = temp.totalMuncherKilled;
        totalBatzKilled = temp.totalBatzKilled;
        totalIronturetKilled = temp.totalIronturetKilled;
        totalTurretsDestroyed = temp.totalTurretsDestroyed;

        enemiesEliminatedLastRound = temp.enemiesEliminatedLastRound;
        goolsKilledLastRound = temp.goolsKilledLastRound;
        shieldKilledLastRound = temp.shieldKilledLastRound;
        muncherKilledLastRound = temp.muncherKilledLastRound;
        batzKilledLastRound = temp.batzKilledLastRound;
        ironturetKilledLastRound = temp.ironturetKilledLastRound;
        turretsDestroyedLastRound = temp.turretsDestroyedLastRound;
        totalAttempts = temp.totalAttempts;
    }
}

[System.Serializable]
public class TempAchievements
{
    public int highestDepthReached = 0;
    public int lastSDepth = 0;

    public int totalEnemiesKilled = 0;
    public int totalGoolsKilled = 0;
    public int totalShieldKilled = 0;
    public int totalMuncherKilled = 0;
    public int totalBatzKilled = 0;
    public int totalIronturetKilled = 0;
    public int totalTurretsDestroyed = 0;

    public int enemiesEliminatedLastRound = 0;
    public int goolsKilledLastRound = 0;
    public int shieldKilledLastRound = 0;
    public int muncherKilledLastRound = 0;
    public int batzKilledLastRound = 0;
    public int ironturetKilledLastRound = 0;
    public int turretsDestroyedLastRound = 0;

    public int totalAttempts = 0;
}