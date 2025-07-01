using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    public Challenge[] challenges;
    public List<int> possibleChallenges;

    private Challenge currentChallenge;

    private void Awake()
    {
        challenges = GetComponentsInChildren<Challenge>();
        possibleChallenges = new List<int>(challenges.Length);

        ResetPossibleChallenges();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ChooseChallenge();
        if (Input.GetKeyDown(KeyCode.V))
            StartChallenge();
    }

    private void StartChallenge()
    {
        if ( currentChallenge == null)
        {
            Debug.Log("No Current Challenge");
            return;
        }

        currentChallenge.StartChallenge();
    }

    public void ChooseChallenge()
    {
        int index = Random.Range(0, possibleChallenges.Count);
        int challengeIndex = possibleChallenges[index];

        currentChallenge = challenges[challengeIndex];
        currentChallenge.InitializeChallenge();

        currentChallenge.OnChallengeCompleted += HandleChallengeSuccess;
        currentChallenge.OnChallengeFailed += HandleChallengeFailure;

        possibleChallenges.Remove(challengeIndex);

        if (possibleChallenges.Count == 0)
            ResetPossibleChallenges();
    }

    private void HandleChallengeSuccess()
    {
        // Instantiate door and level upgrade

        DungeonManager.Instance.HandleLevelCompletion();

        currentChallenge.OnChallengeCompleted -= HandleChallengeSuccess;
        currentChallenge.OnChallengeFailed -= HandleChallengeFailure;
    }

    private void HandleChallengeFailure()
    {
        // Exit Core from dungeon
        
        currentChallenge.OnChallengeCompleted -= HandleChallengeSuccess;
        currentChallenge.OnChallengeFailed -= HandleChallengeFailure;
    }

    private void ResetPossibleChallenges()
    {
        possibleChallenges.Clear();

        for (int i = 0; i < challenges.Length; i++)
            possibleChallenges.Add(i);
    }
}
