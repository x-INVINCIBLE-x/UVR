using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    public static ChallengeManager instance;
    public Challenge[] challenges;
    public List<int> possibleChallenges;

    public Challenge CurrentChallenge { get; private set; }

    public event System.Action<Challenge> OnChallengeChoosen;
    public event System.Action<ChallengeType> OnChallengeStart;
    public event System.Action OnChallengeSuccess;
    public event System.Action OnChallengeFail;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            Destroy(gameObject);

        if (challenges == null || challenges.Length == 0)
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
        if ( CurrentChallenge == null)
        {
            Debug.Log("No Current Challenge");
            return;
        }
        
        CurrentChallenge.StartChallenge();
        OnChallengeStart?.Invoke(CurrentChallenge.Type);
    }

    public void ChooseChallenge()
    {
        int index = Random.Range(0, possibleChallenges.Count);
        int challengeIndex = possibleChallenges[index];

        CurrentChallenge = challenges[challengeIndex];
        CurrentChallenge.InitializeChallenge();

        OnChallengeChoosen?.Invoke(CurrentChallenge);

        CurrentChallenge.OnChallengeCompleted += HandleChallengeSuccess;
        CurrentChallenge.OnChallengeFailed += HandleChallengeFailure;

        possibleChallenges.Remove(challengeIndex);

        if (possibleChallenges.Count == 0)
            ResetPossibleChallenges();
    }

    private void HandleChallengeSuccess()
    {
        // Instantiate door and level upgrade

        OnChallengeSuccess?.Invoke();

        ChooseChallenge();
        CurrentChallenge.OnChallengeCompleted -= HandleChallengeSuccess;
        CurrentChallenge.OnChallengeFailed -= HandleChallengeFailure;
    }

    private void HandleChallengeFailure()
    {
        // Exit Core from dungeon
        OnChallengeFail?.Invoke();
        
        CurrentChallenge.OnChallengeCompleted -= HandleChallengeSuccess;
        CurrentChallenge.OnChallengeFailed -= HandleChallengeFailure;
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
