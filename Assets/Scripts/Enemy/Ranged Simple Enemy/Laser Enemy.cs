using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class LaserEnemy : SimpleEnemyBase
{
    public GameObject LaserVFX;
    public Transform projectileSpawnPosition;
    public Vector3 LaserStartOffset;

    private LineRenderer laserRenderer;
    private GameObject currentLaser;
    private Coroutine laserRoutine;




    protected override void Start()
    {
        base.Start();
        
    }

    protected override void Update()
    {
        base.Update();

        if (!playerInSightRange && !playerInAttackRange)
        {
            try
            {
                agent.SetDestination(transform.position);
            }
            catch
            {
                Debug.Log(gameObject.name + "has destination errors");
            }
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
            PerformLaserAttack();
        }

        UpdateLaser();
    }

    private void PerformLaserAttack()
    {
        if (currentLaser != null) return;

        VFXManager.ActivateMagicCircle();
        currentLaser = Instantiate(LaserVFX, projectileSpawnPosition);
        laserRenderer = currentLaser.GetComponent<LineRenderer>();
        laserRenderer.SetPosition(0, transform.position);
        laserRenderer.SetPosition(1, Player.position);

        if (laserRoutine != null) StopCoroutine(laserRoutine);
        laserRoutine = StartCoroutine(UpdateLaserPosition());
    }

    private void DestroyLaser()
    {
        if (currentLaser != null)
        {
            Destroy(currentLaser);
            currentLaser = null;
            VFXManager.DestroyMagicCircle();
        }
    }

    private void UpdateLaser()
    {
        if (laserRenderer != null)
        {
            laserRenderer.SetPosition(0, projectileSpawnPosition.position);
            laserRenderer.SetPosition(1, Player.position + PlayerBodyOffset);
        }
    }

    private System.Collections.IEnumerator UpdateLaserPosition()
    {
        Vector3 startOffset = (Player.position) + LaserStartOffset;
        float duration = 2f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            Vector3 interpolated = Vector3.Lerp(startOffset, Player.position + PlayerBodyOffset, time / duration);
            laserRenderer.SetPosition(1, interpolated);
            yield return null;
        }
    }

    protected override void Patrol()
    {
        base.Patrol();
    }

    protected override void SearchWalkPoint()
    {
        base.SearchWalkPoint();
    }

    protected override void Chase()
    {
        base.Chase();
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
    }
}
