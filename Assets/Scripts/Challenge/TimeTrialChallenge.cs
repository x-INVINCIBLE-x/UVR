using Autodesk.Fbx;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum QuestTarget
{
    Melee,
    Ranged,
    Crystal,
    Statue
}

public class TimeTrialChallenge : Challenge
{
    [SerializeField] private int targetAmount;
    private List<int> possibleTargets = new();

    private QuestTarget currentTarget;
    

    public override void InitializeChallenge()
    {
        if (possibleTargets.Count != 0) { return; }

        ResetTargets();
    }

    public override void StartChallenge()
    {
        int targetIndex = UnityEngine.Random.Range(0, possibleTargets.Count);
        currentTarget = (QuestTarget)possibleTargets[targetIndex];

        possibleTargets.RemoveAt(targetIndex);

    }

    public override void ChallengeCompleted()
    {

    }

    public override void ChallengeFailed()
    {

    }

    private void ResetTargets()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Target)).Length; i++)
        {
            possibleTargets.Add(i);
        }
    }
}
