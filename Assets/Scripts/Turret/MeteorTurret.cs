using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorTurret : AttackTurret
{
    [SerializeField] private Transform spawnPointsParent;

    [Header("Projectile Info")]
    [SerializeField] private HomingMissile projectilePrefab;
    [SerializeField] private AttackData attackData;
    [SerializeField] private float upSpeed;
    [SerializeField] private float upDuration;
    [SerializeField] private float downSpeed;
    [SerializeField] private float acceleration;

    [Header("Timers")]
    [SerializeField] private float timeBetweenAttacks = 1f;
    [SerializeField] private float homingDuration;
    [SerializeField] private float projectileLifetime;

    private Transform[] spawnPortals; // Must have odd amount
    private Rigidbody target;
    private Coroutine currentRoutine = null;
    private List<HomingMissile> activeProjectiles = new();
    private bool isAttacking = false;

    protected override void OnPreAttackEnter()
    {
        base.OnPreAttackEnter();

        if (spawnPortals == null || spawnPortals.Length == 0)
        {
            spawnPortals = new Transform[spawnPointsParent.childCount];
            for (int i = 0; i < spawnPointsParent.childCount; i++)
            {
                spawnPortals[i] = spawnPointsParent.GetChild(i);
            }
        }

        target = PlayerManager.instance.Rb;
    }

    protected override void OnPreAttackExit()
    {
        base.OnPreAttackExit();
    }

    protected override void OnAttackEnter()
    {
        base.OnAttackEnter();

        isAttacking = true;

        if (currentRoutine != null) 
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(AttackStartRoutine());
    }

    private IEnumerator AttackStartRoutine()
    {
        while (isAttacking)
        {
            // Raise the projectiles up from the spawn points
            foreach (Transform spawn in spawnPortals)
            {
                HomingMissile missile = ObjectPool.instance.GetObject(projectilePrefab.gameObject, spawn).
                    GetComponent<HomingMissile>();
                missile.enabled = false;
                activeProjectiles.Add(missile);

                // Move up over time
                Rigidbody rb = missile.GetComponent<Rigidbody>();
                rb.linearVelocity = Vector3.up * upSpeed;

                yield return new WaitForSeconds(timeBetweenAttacks);
            }

            float waitTime = Mathf.Max(0, upDuration - (timeBetweenAttacks * spawnPortals.Length));
            yield return new WaitForSeconds(waitTime);

            // Set projectiles to track the target now
            foreach (HomingMissile missile in activeProjectiles)
            {
                if (missile != null)
                {
                    // Now setup tracking for homing
                    missile.Setup(target, attackData, downSpeed, acceleration, homingDuration, projectileLifetime);
                    missile.enabled = true;
                }
            }

            activeProjectiles.Clear();
        }

        currentRoutine = null;
    }

    protected override void OnAttackExit()
    {
        base.OnAttackExit();

        isAttacking = false;
    }

    protected override void OnCooldownEnter()
    {
        base.OnCooldownEnter();
    }

    protected override void OnCooldownExit()
    {
        base.OnCooldownExit();
    }
}
