using System.Collections;
using UnityEngine;

public class LaserEnemy : SimpleEnemyBase
{
    public GameObject LaserVFX;
    public Transform projectileSpawn;
    public Vector3 LaserStartOffset;

    private LineRenderer laserRenderer;
    private GameObject currentLaser;
    private Coroutine laserRoutine;
    private bool isCharging = false;

    protected override void Update()
    {
        base.Update();

        if (isDead) return;

        if (!playerInSightRange && !playerInAttackRange)
        {
            agent.SetDestination(transform.position);
            if (laserRoutine != null) StopCoroutine(laserRoutine);
            DestroyLaser();
            Patrol();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            Chase();
            if (laserRoutine != null) StopCoroutine(laserRoutine);
            DestroyLaser();
        }
        else if (playerInAttackRange)
        {
            agent.SetDestination(transform.position);
            transform.LookAt(Player.position + PlayerBodyOffset);

            // Only start charging if not already attacking
            if (currentLaser == null && !isCharging)
            {
                Attack();
            }
        }

        UpdateLaser();
    }


    protected override void Attack()
    {
        base.Attack();
        Invoke(nameof(AttackStart), 2f);
    }

    private void AttackStart()
    {
        StartCoroutine(ChargeAndFireLaser());
    }

    private IEnumerator ChargeAndFireLaser()
    {
        isCharging = true;

        // Spawning the magic circle VFX
        if (!vfxSpawned)
        {
            FXManager.SpawnMagicCircleVFX(MagicChargeTime);
            vfxSpawned = true;
        }

        yield return new WaitForSeconds(MagicChargeTime);

        // Spawn laser after charging
        if (currentLaser != null)
            DestroyLaser();

        currentLaser = Instantiate(LaserVFX, projectileSpawn);
        laserRenderer = currentLaser.GetComponent<LineRenderer>();
        laserRenderer.SetPosition(0, projectileSpawn.position);
        laserRenderer.SetPosition(1, Player.position + PlayerBodyOffset);

        // Laser Coroutine for tracking the laser to player
        if (laserRoutine != null) StopCoroutine(laserRoutine);
        laserRoutine = StartCoroutine(UpdateLaserPosition());

        isCharging = false;
        vfxSpawned = false;
    }

    private void DestroyLaser()
    {
        if (currentLaser != null)
        {
            Destroy(currentLaser);
            currentLaser = null;
        }
        FXManager.DestroyMagicCircleVFX();
        isCharging = false;
        vfxSpawned = false;
    }

    private void UpdateLaser()
    {
        if (laserRenderer != null)
        {
            laserRenderer.SetPosition(0, projectileSpawn.position);
            laserRenderer.SetPosition(1, Player.position + PlayerBodyOffset);
        }
    }

    private IEnumerator UpdateLaserPosition()
    {
        Vector3 startOffset = Player.position + LaserStartOffset;
        float duration = 2f; // can be used to increase the duration for laser to lerp and reach the player
        float time = 0f; //  timer intialization

        while (time < duration)
        {
            time += Time.deltaTime;
            Vector3 interpolated = Vector3.Lerp(startOffset, Player.position + PlayerBodyOffset, time / duration);
            laserRenderer.SetPosition(1, interpolated);
            yield return null;
        }
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();

        if (currentLaser != null)
        {
            DestroyLaser();
        }
    }
}