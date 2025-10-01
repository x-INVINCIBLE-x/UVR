using Autodesk.Fbx;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class LaserEnemy : SimpleEnemyBase
{
    public GameObject LaserVFX;
    public Transform projectileSpawn;
    public Vector3 LaserStartOffset;
    public float laserSetTime = 2f; // can be used to increase the duration for laser to lerp and reach the player
    public CapsuleCollider laserCollider;

    private LineRenderer laserRenderer;
    private GameObject currentLaser;
    private Coroutine laserRoutine = null;
    private Coroutine chargeRoutine = null;
    private bool isCharging = false;
    private bool shouldUpdateLaser = false;

    protected override void Update()
    {
        base.Update();

        if (isDead) return;

        if (!playerInSightRange && !playerInAttackRange)
        {
            agent.SetDestination(transform.position);
            if (laserRoutine != null) StopRoutines();
            DestroyLaser();
            Patrol();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            Chase();
            if (laserRoutine != null) StopRoutines();
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

    private void StopRoutines()
    {
        StopCoroutine(laserRoutine);
        if (chargeRoutine != null)
        {
            StopCoroutine(chargeRoutine);
            chargeRoutine = null;
        }
    }

    protected override void Attack()
    {
        base.Attack();
        Invoke(nameof(AttackStart), 2f);
    }

    private void AttackStart()
    {
        if (chargeRoutine != null) return;
        chargeRoutine = StartCoroutine(ChargeAndFireLaser());
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

        laserCollider.gameObject.SetActive(true);

        currentLaser = Instantiate(LaserVFX, projectileSpawn);
        laserRenderer = currentLaser.GetComponent<LineRenderer>();
        laserRenderer.SetPosition(0, projectileSpawn.position);
        laserRenderer.SetPosition(1, projectileSpawn.position);

        // Laser Coroutine for tracking the laser to player
        if (laserRoutine != null) StopCoroutine(laserRoutine);
        laserRoutine = StartCoroutine(UpdateLaserPosition());

        isCharging = false;
        vfxSpawned = false;
        chargeRoutine = null;
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
        laserCollider.gameObject.SetActive(false);
    }

    private void UpdateLaser()
    {
        if (shouldUpdateLaser && laserRenderer != null)
        {
            laserRenderer.SetPosition(0, projectileSpawn.position);
            laserRenderer.SetPosition(1, Player.position + PlayerBodyOffset);
            laserCollider.transform.position = (projectileSpawn.position + Player.position + PlayerBodyOffset) / 2;
            laserCollider.height = Vector3.Distance(projectileSpawn.position, Player.position + PlayerBodyOffset) * 2.2f;
        }
    }

    private IEnumerator UpdateLaserPosition()
    {
        shouldUpdateLaser = false;
        Vector3 startOffset = Player.position + LaserStartOffset;
        float time = 0f; //  timer intialization

        while (time < laserSetTime)
        {
            time += Time.deltaTime;

            Vector3 startPoint = projectileSpawn.position;
            Vector3 endPoint = Vector3.Lerp(startOffset, Player.position + PlayerBodyOffset, time / laserSetTime);

            laserRenderer.SetPosition(1, endPoint);

            Vector3 midPoint = (startPoint + endPoint) * 0.5f;
            laserCollider.transform.position = midPoint;

            Vector3 direction = (endPoint - startPoint).normalized;
            laserCollider.transform.rotation = Quaternion.LookRotation(direction);

            float length = Vector3.Distance(startPoint, endPoint) * 2.2f;
            laserCollider.height = length;
            

            yield return null;
        }

        shouldUpdateLaser = true;
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