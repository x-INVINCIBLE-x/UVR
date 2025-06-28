using System;
using System.Collections;
using UnityEngine;

public class SurvivalChallenge : Challenge
{
    [SerializeField] private float survivalDuration = 300;
    [SerializeField] private float timer;

    private Coroutine currentRoutine;
    private float tickTime = 1f;
    private bool isChallengeCompleted;

    public override void InitializeChallenge()
    {
        PlayerManager.instance.OnPlayerDeath += ChallengeFailed;

        isChallengeCompleted = false;
        timer = survivalDuration;
    }


    public override void StartChallenge()
    {
        currentRoutine = StartCoroutine(StartChallengeRoutine());
    }

    public override void ChallengeCompleted()
    {
        Debug.Log(challengeName + "Completed");
        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;
    }

    public override void ChallengeFailed()
    {
        if (isChallengeCompleted) return;
        StopCoroutine(currentRoutine);

        Debug.Log(challengeName + "Failed");
        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;
    }

    private IEnumerator StartChallengeRoutine()
    {
        while (timer > 0f)
        {
            yield return new WaitForSeconds(tickTime);

            timer -= tickTime;
        }

        if (timer <= 0f)
        {
            ChallengeCompleted();
            isChallengeCompleted = true;
        }

        currentRoutine = null;
    }
}
