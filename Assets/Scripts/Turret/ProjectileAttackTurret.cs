using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ProjectileAttackTurret : AttackTurret
{
    [SerializeField] private Transform spawnPointsParent;

    [Header("Projectile Info")]
    [SerializeField] private HomingMissile missilePrefab;
    [SerializeField] private AttackData attackData;
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;

    [Header("Timers")]
    [SerializeField] private float timeBetweenAttacks = 1f;
    [SerializeField] private float homingDuration;
    [SerializeField] private float projectileLifetime;

    private Transform[] spawnPortals; // Must have odd amount
    private Rigidbody target;
    private Coroutine currentRoutine;

    protected override void Activate(Collider activatingCollider)
    {
        base.Activate(activatingCollider);
    }

    protected override void Deactivate(Collider deactivatingCollider)
    {
        base.Deactivate(deactivatingCollider);

        for (int i = 0; i < spawnPortals.Length; i++)
            spawnPortals[i].gameObject.SetActive(false);
    }

    protected override void OnPreAttackEnter()
    {
        base.OnPreAttackEnter();

        if (spawnPortals == null || spawnPortals.Length == 0)
        {
            int count = spawnPointsParent.childCount;
            spawnPortals = new Transform[count];
            for (int i = 0; i < count; i++)
            {
                spawnPortals[i] = spawnPointsParent.GetChild(i);
            }
        }

        //Current only Player can be targeted
        if (target == null)
            target = PlayerManager.instance.Rb;

        currentRoutine = StartCoroutine(TargetLookRoutine());

        for (int i = 0; i < spawnPortals.Length; i++)
            spawnPortals[i].gameObject.SetActive(true);
    }

    private IEnumerator TargetLookRoutine()
    {
        while (isActive)
        {
            spawnPointsParent.LookAt(target.position);
            yield return null;
        }
    }
    protected override void OnPreAttackExit()
    {
        base.OnPreAttackExit();
    }

    protected override void OnAttackEnter()
    {
        base.OnAttackEnter();
        currentRoutine = StartCoroutine(StartAttackRoutine());
    }

    private IEnumerator StartAttackRoutine()
    {
        int mid = spawnPortals.Length / 2;

        while (isActive)
        {
            int i = 0;

            if (spawnPortals[mid].gameObject.activeSelf)
                    LaunchMissile(mid);

            while (i <= mid)
            {
                // Always try center

                if (i > 0)
                {
                    int left = mid - i;
                    int right = mid + i;

                    if (left >= 0 && spawnPortals[left].gameObject.activeSelf)
                        LaunchMissile(left);

                    if (right < spawnPortals.Length && spawnPortals[right].gameObject.activeSelf)
                        LaunchMissile(right);
                }

                i++;
                yield return new WaitForSeconds(timeBetweenAttacks);
            }

            yield return null;
        }
    }

    private void LaunchMissile(int i)
    {
        HomingMissile newMissile = ObjectPool.instance.GetObject(missilePrefab.gameObject, spawnPortals[i])
            .GetComponent<HomingMissile>();
        newMissile.transform.position = spawnPortals[i].transform.position;
        newMissile.Setup(target, attackData, speed, acceleration,homingDuration, projectileLifetime);
    }

    protected override void OnAttackExit()
    {
        base.OnAttackExit();

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
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
