using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class LaserEnemy : SimpleEnemyBase
{
    public GameObject LaserVFX;
    public Transform projectileSpawnPosition;
    public Vector3 LaserStartOffset;

    private LineRenderer laserRenderer;
    private GameObject currentLaser;
    private Coroutine laserRoutine;
    private bool isCharging = false;

    protected override void Update()
    {
        base.Update();

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
                StartCoroutine(ChargeAndFireLaser());
            }
        }

        UpdateLaser();
    }

    private IEnumerator ChargeAndFireLaser()
    {
        isCharging = true;

        // Spawning the magic circle VFX
        if (!vfxSpawned)
        {
            VFXManager.SpawnMagicCircleVFX(magicChargeTime);
            vfxSpawned = true;
        }

        yield return new WaitForSeconds(magicChargeTime);

        // Spawn laser after charging
        if (currentLaser != null)
            Destroy(currentLaser);

        currentLaser = Instantiate(LaserVFX, projectileSpawnPosition);
        laserRenderer = currentLaser.GetComponent<LineRenderer>();
        laserRenderer.SetPosition(0, projectileSpawnPosition.position);
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
            VFXManager.DestroyMagicCircleVFX();
        }
        isCharging = false;
        vfxSpawned = false;
    }

    private void UpdateLaser()
    {
        if (laserRenderer != null)
        {
            laserRenderer.SetPosition(0, projectileSpawnPosition.position);
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

   
}