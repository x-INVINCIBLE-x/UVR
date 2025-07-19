using System;
using System.Collections;
using UnityEngine;

public class SurvivalChallenge : Challenge
{
    [SerializeField] private float survivalDuration = 300;
    [SerializeField] private float timer;

    private Coroutine currentRoutine;
    private const float TickTime = 1f;

    private void Awake()
    {
        technicalDetails = $"SURVIVE for {survivalDuration} seconds to complete the challenge.";
    }

    public override void InitializeChallenge()
    {
        if (PlayerManager.instance != null)
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

        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;
    }

    public override void ChallengeFailed()
    {
        if (status == ChallengeStatus.Success) return;
        StopCoroutine(currentRoutine);

        base.ChallengeFailed();

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

    public override string GetProgressText()
    {
        string text = "";
        if (status == ChallengeStatus.InProgress)
        {
            text = $"Survive for : {Mathf.RoundToInt(timer)}";
        }
        else if (status == ChallengeStatus.Success)
        {
            text = "Challenege Completed";
        }
        else
        {
            text = "Challenge Failed";
        }

        return text;
    }

    private void OnDestroy()
    {
        if (PlayerManager.instance != null)
            PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;
    }
}
