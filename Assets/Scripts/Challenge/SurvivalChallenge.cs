using System;
using System.Collections;
using UnityEngine;

public class SurvivalChallenge : Challenge
{
    [SerializeField] private float survivalDuration = 300;
    [SerializeField] private float timer;

    private Coroutine currentRoutine;
    private const float TickTime = 1f;

    public override void InitializeChallenge()
    {
        PlayerManager.instance.OnPlayerDeath += ChallengeFailed;

        status = ChallengeStatus.InProgress;
        timer = survivalDuration;
    }


    public override void StartChallenge()
    {
        currentRoutine = StartCoroutine(StartChallengeRoutine());
    }

    public override void ChallengeCompleted()
    {
        if (status == ChallengeStatus.Failed)
            return;

        base.ChallengeCompleted();

        Debug.Log(ChallengeName + " Completed");
        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;
    }

    public override void ChallengeFailed()
    {
        if (status == ChallengeStatus.Success) return;
        StopCoroutine(currentRoutine);

        base.ChallengeFailed();

        Debug.Log(ChallengeName + " Failed");
        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;
    }

    private IEnumerator StartChallengeRoutine()
    {
        while (timer > 0f)
        {
            yield return new WaitForSeconds(TickTime);

            timer -= TickTime;
        }

        if (timer <= 0f)
        {
            ChallengeCompleted();
            status = ChallengeStatus.Success;
        }

        currentRoutine = null;
    }
}
