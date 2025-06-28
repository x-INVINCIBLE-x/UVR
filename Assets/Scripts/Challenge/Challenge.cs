using UnityEngine;

public abstract class Challenge: MonoBehaviour
{
    [field: SerializeField] public string challengeName { get; private set; }

    public abstract void InitializeChallenge();
    public abstract void StartChallenge();
    public abstract void ChallengeCompleted();
    public abstract void ChallengeFailed();
}
