using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum ObjectiveType
{
    StartCrystal,
    MeleeEnemy,
    RangedEnemy,
    Turret,
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

    [SerializeField] private ObjectiveType currentObjective;
    private float timer;
    private const int TickTime = 1;
    private int currentAmount = 0;
    private Coroutine currentRoutine;
    private string objectiveString ="";

    private void Start()
    {
        objectiveString = currentObjective.ToString();
        StringBuilder ob = new();

        for (int i = 0; i < objectiveString.Length; i++)
        {
            char c = objectiveString[i];

            if (i > 0 && char.IsUpper(c))
                ob.Append(' ');

            ob.Append(c);
        }

        objectiveString = ob.ToString();

        technicalDetails = $"ELIMINATE {targetAmount} {ob} in {challengeDuration} seconds to complete the challenge. \n\n Each Elimination will add a BONUS TIME of {bonusTime} seconds";
    }

    public override void InitializeChallenge()
    {
        status = ChallengeStatus.InProgress;
        timer = challengeDuration;

        if (possibleTargets.Count != 0) { return; }
        currentAmount = 0;
        ResetTargets();
    }

    public override void StartChallenge()
    {
        GameEvents.OnElimination += UpdateChallengeStatus;
        int targetIndex = UnityEngine.Random.Range(0, possibleTargets.Count);
        currentObjective = (ObjectiveType)possibleTargets[targetIndex];

        possibleTargets.RemoveAt(targetIndex);
        currentRoutine = StartCoroutine(StartChallengeRoutine());
    }

    public override void ChallengeCompleted()
    {
        if (status == ChallengeStatus.Failed) return;

        status = ChallengeStatus.Success;
        GameEvents.OnElimination -= UpdateChallengeStatus;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        base.ChallengeCompleted();
    }

    public override void ChallengeFailed()
    {
        if (status == ChallengeStatus.Success) return;

        status = ChallengeStatus.Failed;
        GameEvents.OnElimination -= UpdateChallengeStatus;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        base.ChallengeFailed();
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

    public override string GetProgressText()
    {
        string text = "";

        if (status == ChallengeStatus.InProgress)
        {
            text = $"Eliminate {currentAmount} / {targetAmount} {objectiveString} \n in : {Mathf.RoundToInt(timer)} seconds";
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
}
