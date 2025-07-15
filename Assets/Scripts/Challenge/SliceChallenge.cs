using Unity.VisualScripting;
using UnityEngine;

public class SliceChallenge : Challenge
{
    [SerializeField] private int totalCrystals = 3;
    [SerializeField] private GameObject statueShield;

    private int activeCrystals;

    private void Awake()
    {
        technicalDetails = $"Destroy the 3 CRYSTALS and finally the STATUE to complete the challenge.";
    }

    public override void InitializeChallenge()
    {
        status = ChallengeStatus.InProgress;
        activeCrystals = totalCrystals;
        
        statueShield.SetActive(true);
    }

    public override void StartChallenge()
    {
        GameEvents.OnElimination += HandleElimination;
        PlayerManager.instance.OnPlayerDeath += ChallengeFailed;
    }

    public override void ChallengeCompleted()
    {
        if (status == ChallengeStatus.Failed)
            return;

        status = ChallengeStatus.Success;

        GameEvents.OnElimination -= HandleElimination;
        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;

        base.ChallengeCompleted();
    }

    public override void ChallengeFailed()
    {
        if (status == ChallengeStatus.Success)
            return;

        status = ChallengeStatus.Failed;

        GameEvents.OnElimination -= HandleElimination;
        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;

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
}
