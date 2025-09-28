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
    [System.Serializable]
    private class DifficultyScaling
    {
        public int duration;
        public int targetAmount;
    }

    [SerializeField] private int baseTargetAmount;
    [SerializeField] private int baseChallengeDuration;
    [Tooltip("Added Extra Time for getting closrt to objective")]
    [SerializeField] private float bonusTime;

    [SerializeField] private ObjectiveType currentObjective;
    [SerializeField] private List<ObjectiveType> targetObjectives;
    private List<ObjectiveType> possibleTargets = new();

    private float timer;
    private const int TickTime = 1;
    private int currentAmount = 0;
    private Coroutine currentRoutine;
    private string objectiveString ="";

    private int targetAmount;
    private int challengeDuration;
    [SerializeField] private DifficultyScaling difficultyScaling;
    [SerializeField] private DifficultyScaling scalingCap;

    public override void InitializeChallenge(int level)
    {
        int scalingFactor = level / difficultyStep;
        challengeDuration = Mathf.Min(scalingCap.duration, baseChallengeDuration + (difficultyScaling.duration * scalingFactor));
        targetAmount = Mathf.Min(scalingCap.targetAmount, baseTargetAmount + (difficultyScaling.targetAmount * scalingFactor));

        status = ChallengeStatus.InProgress;
        timer = challengeDuration;
        currentAmount = 0;

        if (possibleTargets.Count == 0)
        {
            ResetTargets();
        }

        int targetIndex = UnityEngine.Random.Range(0, possibleTargets.Count);
        currentObjective = possibleTargets[targetIndex];

        UpdateConditionText();

        possibleTargets.Remove(currentObjective);
    }

    public override void StartChallenge()
    {
        base.StartChallenge();
        GameEvents.OnElimination += UpdateChallengeStatus;
        PlayerManager.instance.OnPlayerDeath += ChallengeFailed;
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
        Debug.Log("Failed");
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

        if (currentAmount == targetAmount)
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
        for (int i = 0; i < targetObjectives.Count; i++)
        {
            possibleTargets.Add(targetObjectives[i]);
        }
    }

    private void UpdateConditionText()
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
        possibleTargets.Clear();
        technicalDetails = $"ELIMINATE {targetAmount} {ob} in {challengeDuration} seconds to complete the challenge. \n\n Each Elimination will add a BONUS TIME of {bonusTime} seconds";
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
