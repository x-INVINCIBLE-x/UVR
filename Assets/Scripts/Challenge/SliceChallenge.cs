using Unity.VisualScripting;
using UnityEngine;

public class SliceChallenge : Challenge
{
    [SerializeField] private int totalCrystals = 3;
    [SerializeField] private GameObject statueShield;

    private int activeCrystals;

    private void Awake()
    {
        ChallengeName = "Slice";
    }

    public override void InitializeChallenge()
    {
        status = ChallengeStatus.InProgress;
        activeCrystals = totalCrystals;
        
        statueShield.SetActive(true);
    }

    public override void StartChallenge()
    {
        EnemyEvents.OnElimination += HandleElimination;
        PlayerManager.instance.OnPlayerDeath += ChallengeFailed;
    }

    public override void ChallengeCompleted()
    {
        if (status == ChallengeStatus.Failed)
            return;

        status = ChallengeStatus.Success;

        Debug.Log(ChallengeName + " Completed");
        EnemyEvents.OnElimination -= HandleElimination;
        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;

        base.ChallengeCompleted();
    }

    public override void ChallengeFailed()
    {
        if (status == ChallengeStatus.Success)
            return;

        status = ChallengeStatus.Failed;

        Debug.Log(ChallengeName + " Completed");
        EnemyEvents.OnElimination -= HandleElimination;
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
