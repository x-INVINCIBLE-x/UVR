using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    public static ChallengeManager instance;
    public Challenge[] challenges;
    public List<int> possibleChallenges;

    private Challenge currentChallenge;

    public event System.Action<string> OnChallengeStart;
    public event System.Action OnChallengeSuccess;
    public event System.Action OnChallengeFail;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            Destroy(gameObject);

        challenges = GetComponentsInChildren<Challenge>();
    }

    private void Start()
    {
        GameEvents.OnElimination += HandleChallengeStart;
        possibleChallenges = new List<int>(challenges.Length);
        ResetPossibleChallenges();
        ChooseChallenge();
    }

    private void HandleChallengeStart(ObjectiveType type)
    {
        if (type == ObjectiveType.StartCrystal)
        {
            StartChallenge();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ChooseChallenge();
        if (Input.GetKeyDown(KeyCode.V))
            StartChallenge();
    }

    // Starting Sliceable Crystal must be marked as StartCrystal to Start Challenge
    private void StartChallenge()
    {
        if ( currentChallenge == null)
        {
            Debug.Log("No Current Challenge");
            return;
        }

        currentChallenge.StartChallenge();
        OnChallengeStart?.Invoke(currentChallenge.ChallengeName);
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

        OnChallengeSuccess?.Invoke();

        currentChallenge.OnChallengeCompleted -= HandleChallengeSuccess;
        currentChallenge.OnChallengeFailed -= HandleChallengeFailure;
    }

    private void HandleChallengeFailure()
    {
        // Exit Core from dungeon
        OnChallengeFail?.Invoke();
        
        currentChallenge.OnChallengeCompleted -= HandleChallengeSuccess;
        currentChallenge.OnChallengeFailed -= HandleChallengeFailure;
    }

    private void ResetPossibleChallenges()
    {
        possibleChallenges.Clear();

        for (int i = 0; i < challenges.Length; i++)
            possibleChallenges.Add(i);
    }

    private void OnDestroy()
    {
        GameEvents.OnElimination -= HandleChallengeStart;
    }
}
