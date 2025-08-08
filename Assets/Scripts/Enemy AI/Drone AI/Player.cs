using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public Transform playerBody { get; internal set; }
    public PlayerStats stats { get; private set; }
    public CharacterXP characterXP { get; private set; }

    [field: SerializeField] private PlayerGravity playerGravity;

    [Header("Safe Position")]
    [field: SerializeField] private float updateTimer = 2f;
    private float safePositionTimer = 0f;
    [SerializeField] private Vector3 safePositionOffset = Vector3.zero;
    [SerializeField] private Vector3 lastSafePosition;

    private Coroutine positionResetRoutine = null;

    private void Awake()
    {
        stats = GetComponentInChildren<PlayerStats>();
        characterXP = GetComponentInChildren<CharacterXP>();
        playerGravity = GetComponentInChildren<PlayerGravity>();
    }

    private void Start()
    {
        characterXP.OnLevelUp += stats.OnLevelUp;

        StartCoroutine(SafePositionRoutine());
    }

    public void SetPlayerToSafePosition()
    {
        if (positionResetRoutine != null) 
            return;
        
        positionResetRoutine = StartCoroutine(SetToSafePosition());
    }

    private IEnumerator SetToSafePosition()
    {
        PlayerManager.instance.ActionMediator.DisableControl();
        yield return new WaitForSeconds(0.1f); // Small delay to ensure control is disabled
        Debug.Log("Setting player to last safe position: " + lastSafePosition);
        PlayerManager.instance.SetPlayerPosition(lastSafePosition + safePositionOffset);
        yield return new WaitForSeconds(0.1f); // Small delay to ensure position is set

        // Recurse this function if still giving problems

        PlayerManager.instance.ActionMediator.EnableControl();
    }

    private IEnumerator SafePositionRoutine()
    {
        while (true)
        {
            while (safePositionTimer > 0f)
            {
                safePositionTimer -= Time.deltaTime;
                yield return null;
            }

            yield return new WaitUntil(() => playerGravity.IsGrounded());

            lastSafePosition = PlayerManager.instance.PlayerOrigin.transform.position;
            safePositionTimer = updateTimer;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(lastSafePosition, 0.2f);
    }

    private void OnDestroy()
    {
        characterXP.OnLevelUp -= stats.OnLevelUp;

        StopAllCoroutines();
    }
}