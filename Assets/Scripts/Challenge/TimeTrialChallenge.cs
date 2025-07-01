using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveType
{
    MeleeEnemy,
    RangedEnemy,
    Crystal,
    Statue
}

public class TimeTrialChallenge : Challenge
{
    [SerializeField] private int targetAmount;
    [SerializeField] private int challengeDuration;
    [Tooltip("Added Extra Time for getting closrt to objective")]
    [SerializeField] private float bonusTime;
    private List<int> possibleTargets = new();

    private ObjectiveType currentObjective;
    private float timer;
    private const int TickTime = 1;
    private int currentAmount = 0;
    private Coroutine currentRoutine;

    public override void InitializeChallenge()
    {
        status = ChallengeStatus.InProgress;
        if (possibleTargets.Count != 0) { return; }

        currentAmount = 0;
        ResetTargets();
    }

    public override void StartChallenge()
    {
        EnemyEvents.OnElimination += UpdateChallengeStatus;
        int targetIndex = UnityEngine.Random.Range(0, possibleTargets.Count);
        currentObjective = (ObjectiveType)possibleTargets[targetIndex];

        possibleTargets.RemoveAt(targetIndex);
        currentRoutine = StartCoroutine(StartChallengeRoutine());
    }

    public override void ChallengeCompleted()
    {
        if (status == ChallengeStatus.Failed) return;

        status = ChallengeStatus.Success;
        EnemyEvents.OnElimination -= UpdateChallengeStatus;

        Debug.Log(ChallengeName + " Completed");
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

    }

    public override void ChallengeFailed()
    {
        if (status == ChallengeStatus.Success) return;

        status = ChallengeStatus.Failed;
        EnemyEvents.OnElimination -= UpdateChallengeStatus;

        Debug.Log(ChallengeName + " Failed");
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
    }

    private void UpdateChallengeStatus(ObjectiveType type)
    {
        if (type != currentObjective)
            return;

        currentAmount++;
        timer += bonusTime;

        if (currentAmount == possibleTargets.Count)
        {
            ChallengeCompleted();
        }
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
            if (status != ChallengeStatus.Success)
                ChallengeFailed();
        }

        currentRoutine = null;
    }

    private void ResetTargets()
    {
        for (int i = 0; i < Enum.GetValues(typeof(ObjectiveType)).Length; i++)
        {
            possibleTargets.Add(i);
        }
    }
}
