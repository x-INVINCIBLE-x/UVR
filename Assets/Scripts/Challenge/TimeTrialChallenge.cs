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
    JackOGools,
    GoblinShield,
    FellironTurret,
    EmeraldBats,
    Turret,
    Crystal,
    Statue
}

[System.Serializable]
public class ObjectiveSettings
{
    public ObjectiveType objectiveType;

    [Header("Base Values")]
    public int baseTargetAmount;
    public int baseChallengeDuration;

    [Header("Scaling Per Level")]
    public int targetAmountScaling;
    public int durationScaling;

    [Header("Scaling Cap")]
    public int maxTargetAmount;
    public int maxDuration;
}

public class TimeTrialChallenge : Challenge
{
    [System.Serializable]
    private class DifficultyScaling
    {
        public int duration;
        public int targetAmount;
    }

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

    [SerializeField] private List<ObjectiveSettings> objectiveSettingsList = new();
    /// <summary>
    /// Scaling directly add that value on level change
    /// </summary>
    /// <param name="level"></param>
    public override void InitializeChallenge(int level)
    {
        int scalingFactor = level / difficultyStep;

        if (possibleTargets.Count == 0)
        {
            ResetTargets();
        }

        // pick objective
        int targetIndex = UnityEngine.Random.Range(0, possibleTargets.Count);
        currentObjective = possibleTargets[targetIndex];
        possibleTargets.Remove(currentObjective);

        // find settings for this objective
        ObjectiveSettings settings = objectiveSettingsList.Find(s => s.objectiveType == currentObjective);
        if (settings == null)
        {
            Debug.LogError($"No settings defined for objective {currentObjective}");
            return;
        }

        // apply scaling and caps
        challengeDuration = Mathf.Min(
            settings.maxDuration,
            settings.baseChallengeDuration + (settings.durationScaling * scalingFactor)
        );

        targetAmount = Mathf.Min(
            settings.maxTargetAmount,
            settings.baseTargetAmount + (settings.targetAmountScaling * scalingFactor)
        );

        status = ChallengeStatus.InProgress;
        timer = challengeDuration;
        currentAmount = 0;

        UpdateConditionText();
    }


    public override void StartChallenge()
    {
        base.StartChallenge();
        status = ChallengeStatus.InProgress;
        GameEvents.OnElimination += UpdateChallengeStatus;
        PlayerManager.instance.OnPlayerDeath += ChallengeFailed;
        currentRoutine = StartCoroutine(StartChallengeRoutine());
    }

    public override void ChallengeCompleted()
    {
        if (status == ChallengeStatus.Failed) return;

        status = ChallengeStatus.Success;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        base.ChallengeCompleted();
        GameEvents.OnElimination -= UpdateChallengeStatus;
    }

    public override void ChallengeFailed()
    {
        if (status == ChallengeStatus.Success) return;
        status = ChallengeStatus.Failed;
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        Debug.Log("TT called");
        base.ChallengeFailed();
        GameEvents.OnElimination -= UpdateChallengeStatus;
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
