using System;
using System.Collections;
using System.Text;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using static UnityEngine.GraphicsBuffer;
public enum ChallengeType
{
    Slice,
    Other
}

public abstract class Challenge: MonoBehaviour
{
    protected enum ChallengeStatus
    {
        Success,
        InProgress,
        Failed,
    }

    [SerializeField] private string challengeID;
    public string GetID() => challengeID;

    [Tooltip("Slice base Challenge will cause Grid Formation to change only on specific item Slice")]
    public ChallengeType Type { get; private set; }
    protected string technicalDetails;

    protected ChallengeStatus status;

    public event System.Action OnChallengeCompleted;
    public event System.Action OnChallengeFailed;

    public abstract void InitializeChallenge();

    public virtual void StartChallenge()
    {
        PlayerManager.instance.OnPlayerDeath += ChallengeFailed;
    }

    public virtual void ChallengeCompleted()
    {
        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;
        OnChallengeCompleted?.Invoke();
    }

    public virtual void ChallengeFailed()
    {
        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;
        StartCoroutine(RaiseChallengeFailRoutine(4f));
    }

    private IEnumerator RaiseChallengeFailRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        OnChallengeFailed?.Invoke();
    }

    public virtual string GetProgressText()
    {
        return "Yet to Implement Progression Text";
    }

    public string GetTechnicalDetail() => technicalDetails;
}
