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

    [Tooltip("After how many level the difficulty of challege will increase")]
    [SerializeField] protected int difficultyStep;

    public abstract void InitializeChallenge(int level);

    public virtual void StartChallenge()
    {
    }

    public virtual void ChallengeCompleted()
    {
        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;
        OnChallengeCompleted?.Invoke();
    }

    public virtual void ChallengeFailed()
    {
        Debug.Log("Base ChallengeFailed() called");
        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;
        Debug.Log("Base ChallengeFailed called");
        StartCoroutine(RaiseChallengeFailRoutine(1f));
    }

    private IEnumerator RaiseChallengeFailRoutine(float time)
    {
        Debug.Log("Challenge Failed Invoked " + time);
        yield return new WaitForSeconds(time);
        Debug.Log("Challenge Failed Invoked");
        OnChallengeFailed?.Invoke();
    }

    public virtual string GetProgressText()
    {
        return "Yet to Implement Progression Text";
    }

    public string GetTechnicalDetail() => technicalDetails;
}
