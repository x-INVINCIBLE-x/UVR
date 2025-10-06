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

    public GameObject shieldPrefab;

    [Header("Optional First Challenge")]
    [Tooltip("If assigned, this will be forced as the first challenge before randomization.")]
    public Challenge preChosenChallenge;

    private bool usedPreChosen = false;
    [SerializeField] private AudioClip winAudio;
    [SerializeField] private AudioClip failAudio;

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
            StartChallenge();
    }

    private void StartChallenge()
    {
        if (CurrentChallenge == null)
        {
            Debug.Log("No Current Challenge");
            return;
        }

        CurrentChallenge.StartChallenge();
        OnChallengeStart?.Invoke(CurrentChallenge.Type);
    }

    public void ChooseChallenge()
    {
        if (preChosenChallenge != null && !usedPreChosen)
        {
            CurrentChallenge = preChosenChallenge;
            usedPreChosen = true;

            int idx = System.Array.IndexOf(challenges, preChosenChallenge);
            if (idx >= 0)
                possibleChallenges.Remove(idx); 

            InitializeCurrent();
            return;
        }

        int index = Random.Range(0, possibleChallenges.Count);
        int challengeIndex = possibleChallenges[index];

        CurrentChallenge = challenges[challengeIndex];
        possibleChallenges.Remove(challengeIndex);

        if (possibleChallenges.Count == 0)
            ResetPossibleChallenges();

        InitializeCurrent();
    }

    private void InitializeCurrent()
    {
        CurrentChallenge.InitializeChallenge(DungeonManager.Instance.DifficultyLevel);
        CurrentChallenge.OnChallengeCompleted += HandleChallengeSuccess;
        CurrentChallenge.OnChallengeFailed += HandleChallengeFailure;
        Debug.Log(CurrentChallenge.name + " Initialized");

        OnChallengeChoosen?.Invoke(CurrentChallenge);
    }

    public void ForceChallengeFail()
    {
        if (CurrentChallenge != null)
            CurrentChallenge.ChallengeFailed();
    }

    private void HandleChallengeSuccess()
    {
        OnChallengeSuccess?.Invoke();

        CurrentChallenge.OnChallengeCompleted -= HandleChallengeSuccess;
        CurrentChallenge.OnChallengeFailed -= HandleChallengeFailure;
        ChooseChallenge();

        if (AudioManager.Instance != null && winAudio != null)
            AudioManager.Instance.PlaySystemSFX(winAudio);
    }

    private void HandleChallengeFailure()
    {
        OnChallengeFail?.Invoke();
        Debug.Log("ChallengeManager  Failed Invoked");
         
        CurrentChallenge.OnChallengeCompleted -= HandleChallengeSuccess;
        CurrentChallenge.OnChallengeFailed -= HandleChallengeFailure;

        if (AudioManager.Instance != null && failAudio != null)
            AudioManager.Instance.PlaySystemSFX(failAudio);
    }

    private void ResetPossibleChallenges()
    {
        possibleChallenges.Clear();

        for (int i = 0; i < challenges.Length; i++)
            possibleChallenges.Add(i);

        // If preChosen was used, allow it back into pool
        if (preChosenChallenge != null && usedPreChosen)
        {
            int idx = System.Array.IndexOf(challenges, preChosenChallenge);
            if (idx >= 0 && !possibleChallenges.Contains(idx))
                possibleChallenges.Add(idx);
        }
    }

    public void RegisterShield(GameObject shield) => shieldPrefab = shield;

    private void OnDestroy()
    {
        GameEvents.OnElimination -= HandleChallengeStart;
    }
}
