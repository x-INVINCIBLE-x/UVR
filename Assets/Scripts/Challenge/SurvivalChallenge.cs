using System.Collections;
using UnityEngine;

public class SurvivalChallenge : Challenge
{
    [SerializeField] private float survivalDuration = 300;
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

    private void Awake()
    {
        technicalDetails = $"SURVIVE for {survivalDuration} seconds to complete the challenge.";
    }

    public override void InitializeChallenge()
    {
        status = ChallengeStatus.InProgress;
        timer = survivalDuration;
        finalMoveSpeed = moveSpeed;
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
        while (true)
        {
            yield return new WaitForEndOfFrame();
            Vector3 currentPos = safeZoneInstance.transform.position;
            Vector3 targetPos;

            do
            {
                // pick a random point within the circle radius around centrePoint
                Vector2 randomCircle = Random.insideUnitCircle * safeRadius;
                targetPos = new Vector3(
                    centrePoint.x + randomCircle.x,
                    currentPos.y,
                    centrePoint.z + randomCircle.y
                );
            } while (Vector3.Distance(currentPos, targetPos) < minMoveDistance);

            Debug.Log($"Safe Zone moving to: {targetPos}");

            // move towards target
            while (Vector3.Distance(safeZoneInstance.transform.position, targetPos) > 0.1f)
            {
                Debug.Log($"Safe Zone moving... Current Pos: {safeZoneInstance.transform.position}, Target Pos: {targetPos}");
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
