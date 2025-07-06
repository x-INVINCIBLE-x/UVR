using System;
using UnityEngine;


public abstract class Challenge: MonoBehaviour
{
    protected enum ChallengeStatus
    {
        Success,
        InProgress,
        Failed,
    }
    [field: SerializeField] public string ChallengeName { get; protected set; }
    protected ChallengeStatus status;

    public event System.Action OnChallengeCompleted;
    public event System.Action OnChallengeFailed;

    public abstract void InitializeChallenge();
    public abstract void StartChallenge();

    public virtual void ChallengeCompleted()
    {
        OnChallengeCompleted?.Invoke();
    }

    public virtual void ChallengeFailed()
    {
        OnChallengeFailed?.Invoke();
    }
}
