using System.Collections;
using UnityEngine;

public class SurvivalChallenge : Challenge
{
    [SerializeField] private float survivalDuration = 300;
    [SerializeField] private float timer;
    [SerializeField] private GameObject safeZone;
    [SerializeField] private Transform safeBounds;

    [Header("Safe Zone Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stopDuration = 3f;
    [SerializeField] private float minMoveDistance = 5f;

    private Coroutine currentRoutine;
    private Coroutine safeZoneRoutine;
    private const float TickTime = 1f;

    private void Awake()
    {
        technicalDetails = $"SURVIVE for {survivalDuration} seconds to complete the challenge.";
    }

    public override void InitializeChallenge()
    {
        if (PlayerManager.instance != null)
            PlayerManager.instance.OnPlayerDeath += ChallengeFailed;

        status = ChallengeStatus.InProgress;
        timer = survivalDuration;
    }

    public override void StartChallenge()
    {
        currentRoutine = StartCoroutine(StartChallengeRoutine());

        if (safeZone != null && safeBounds != null)
        {
            safeZoneRoutine = StartCoroutine(MoveSafeZoneRoutine());
        }
    }

    public override void ChallengeCompleted()
    {
        if (status == ChallengeStatus.Failed)
            return;

        base.ChallengeCompleted();

        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;

        if (safeZoneRoutine != null)
            StopCoroutine(safeZoneRoutine);
    }

    public override void ChallengeFailed()
    {
        if (status == ChallengeStatus.Success) return;

        StopCoroutine(currentRoutine);

        base.ChallengeFailed();

        PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;

        if (safeZoneRoutine != null)
            StopCoroutine(safeZoneRoutine);
    }

    private IEnumerator StartChallengeRoutine()
    {
        while (timer > 0f)
        {
            yield return new WaitForSeconds(TickTime);
            timer -= TickTime;
        }

        if (timer <= 0f)
        {
            ChallengeCompleted();
            status = ChallengeStatus.Success;
        }

        currentRoutine = null;
    }

    private IEnumerator MoveSafeZoneRoutine()
    {
        Vector3 boundsMin = safeBounds.position - safeBounds.localScale / 2f;
        Vector3 boundsMax = safeBounds.position + safeBounds.localScale / 2f;

        while (true)
        {
            Vector3 currentPos = safeZone.transform.position;
            Vector3 targetPos;

            do
            {
                targetPos = new Vector3(
                    Random.Range(boundsMin.x, boundsMax.x),
                    currentPos.y, 
                    Random.Range(boundsMin.z, boundsMax.z)
                );
            } while (Vector3.Distance(currentPos, targetPos) < minMoveDistance);

            
            while (Vector3.Distance(safeZone.transform.position, targetPos) > 0.1f)
            {
                safeZone.transform.position = Vector3.MoveTowards(
                    safeZone.transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            yield return new WaitForSeconds(stopDuration);
        }
    }

    public override string GetProgressText()
    {
        if (status == ChallengeStatus.InProgress)
        {
            return $"Survive for : {Mathf.RoundToInt(timer)}";
        }
        else if (status == ChallengeStatus.Success)
        {
            return "Challenge Completed";
        }
        else
        {
            return "Challenge Failed";
        }
    }

    private void OnDestroy()
    {
        if (PlayerManager.instance != null)
            PlayerManager.instance.OnPlayerDeath -= ChallengeFailed;
    }
}
