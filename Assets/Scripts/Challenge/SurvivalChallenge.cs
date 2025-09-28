using System.Collections;
using UnityEngine;

public class SurvivalChallenge : Challenge
{
    [System.Serializable]
    private class DifficultyScaling
    {
        public int duration;
    }

    [SerializeField] private float baseSurvivalDuration = 180f;
    private float survivalDuration = 300;
    [SerializeField] private float timer;
    [SerializeField] private SafeZone safeZone;

    [Header("Safe Zone Movement Settings")]
    [SerializeField] private AttackData damageOutsideSafeZone;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stopDuration = 3f;
    [SerializeField] private float minMoveDistance = 5f;
    [SerializeField] private float speedIncreaseRate = 0.05f;
    [SerializeField] private float maxSpeed = 20f;
    private float finalMoveSpeed;

    private Coroutine currentRoutine;
    private Coroutine safeZoneRoutine;
    private const float TickTime = 1f;

    private Vector3 centrePoint;
    private float safeRadius;
    private GameObject safeZoneInstance;

    [SerializeField] private DifficultyScaling difficultyScaling;
    [SerializeField] private DifficultyScaling scalingCap;

    public override void InitializeChallenge(int level)
    {
        int scaleFactor = level / difficultyStep;
        survivalDuration = Mathf.Min(scalingCap.duration, baseSurvivalDuration + (difficultyScaling.duration * scaleFactor));

        status = ChallengeStatus.InProgress;
        timer = survivalDuration;
        finalMoveSpeed = moveSpeed;
        technicalDetails = $"SURVIVE for {survivalDuration} seconds to complete the challenge.";
    }

    public override void StartChallenge()
    {
        base.StartChallenge();
        currentRoutine = StartCoroutine(StartChallengeRoutine());

        PlayerManager.instance.OnPlayerDeath += ChallengeFailed;

        centrePoint = FormationHandler.Instance.gridSpawners[0].GetGridCentre();
        safeRadius = FormationHandler.Instance.gridSpawners[0].GetGridRadius();

        if (safeZone != null)
        {
            safeZoneInstance = Instantiate(safeZone.gameObject, PlayerManager.instance.PlayerOrigin.transform.position, Quaternion.identity);
            safeZoneInstance.GetComponent<SafeZone>().Init(damageOutsideSafeZone);
            safeZoneRoutine = StartCoroutine(MoveSafeZoneRoutine());
        }
    }

    public override void ChallengeCompleted()
    {
        if (status == ChallengeStatus.Failed)
            return;

        base.ChallengeCompleted();

        if (safeZoneRoutine != null)
        {
            StopCoroutine(safeZoneRoutine);
            safeZoneInstance.GetComponent<SafeZone>().SetActive(false);
            Destroy(safeZoneInstance);
        }
    }

    public override void ChallengeFailed()
    {
        if (status == ChallengeStatus.Success) return;
        StopCoroutine(currentRoutine);

        base.ChallengeFailed();

        if (safeZoneRoutine != null)
        {
            StopCoroutine(safeZoneRoutine);
            safeZoneInstance.GetComponent<SafeZone>().SetActive(false);
            Destroy(safeZoneInstance);
        }
    }

    private IEnumerator StartChallengeRoutine()
    {
        while (timer > 0f)
        {
            yield return new WaitForSeconds(TickTime);
            timer -= TickTime;

            if (finalMoveSpeed < maxSpeed)
                finalMoveSpeed += speedIncreaseRate;
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
        Vector3 areaCenter = GridGenerator.Instance.GetSafePosition();
        Vector2 areaSize = GridGenerator.Instance.GetSafeArea();
        Quaternion areaRotation = Quaternion.Euler(GridGenerator.Instance.GetSafeRotation());

        while (true)
        {
            yield return new WaitForEndOfFrame();
            Vector3 currentPos = safeZoneInstance.transform.position;
            Vector3 targetPos = currentPos;

            // keep trying until a valid new point is found
            for (int attempts = 0; attempts < 20; attempts++)
            {
                float randomX = Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
                float randomZ = Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f);

                Vector3 localOffset = new Vector3(randomX, 0, randomZ);
                Vector3 candidate = areaCenter + areaRotation * localOffset;
                candidate.y = currentPos.y;

                if (Vector3.Distance(currentPos, candidate) >= minMoveDistance)
                {
                    targetPos = candidate;
                    break;
                }
            }

            Debug.Log($"Safe Zone moving to: {targetPos}");

            // move towards target
            while (Vector3.Distance(safeZoneInstance.transform.position, targetPos) > 0.1f)
            {
                safeZoneInstance.transform.position = Vector3.MoveTowards(
                    safeZoneInstance.transform.position,
                    targetPos,
                    finalMoveSpeed * Time.deltaTime
                );
                yield return null;
            }

            Debug.Log("Safe Zone reached target, stopping...");
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
