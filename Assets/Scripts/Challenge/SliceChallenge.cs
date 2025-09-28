using Unity.VisualScripting;
using UnityEngine;

public class SliceChallenge : Challenge
{
    [SerializeField] private int totalCrystals = 3;
    [SerializeField] private GameObject statueShield;

    private int activeCrystals;

    private void Awake()
    {
        technicalDetails = $"Destroy the {totalCrystals} CRYSTALS and finally THE STATUE to complete the challenge.";
    }

    public override void InitializeChallenge(int level)
    {
        status = ChallengeStatus.InProgress;
        activeCrystals = totalCrystals;
        
        statueShield.SetActive(true);
    }

    public override void StartChallenge()
    {
        base.StartChallenge();
        GameEvents.OnElimination += HandleElimination;
        PlayerManager.instance.OnPlayerDeath += ChallengeFailed;
    }

    public override void ChallengeCompleted()
    {
        if (status == ChallengeStatus.Failed)
            return;

        status = ChallengeStatus.Success;

        GameEvents.OnElimination -= HandleElimination;
        base.ChallengeCompleted();
    }

    public override void ChallengeFailed()
    {
        if (status == ChallengeStatus.Success)
            return;

        status = ChallengeStatus.Failed;

        GameEvents.OnElimination -= HandleElimination;
        base.ChallengeFailed();
    }

    public void HandleElimination(ObjectiveType type)
    {
        if (type == ObjectiveType.Crystal)
        {
            activeCrystals--;

            if (activeCrystals == 0)
            {
                statueShield.SetActive(false);
            }
        }
        else if (type == ObjectiveType.Statue)
        {
            ChallengeCompleted();
        }
    }

    public override string GetProgressText()
    {
        string text = "";

        if (status == ChallengeStatus.InProgress)
        {
            if (activeCrystals > 0)
                text = $"Crystals Destroyed : {totalCrystals - activeCrystals}";
            else
                text = "Destroy Statue";
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
