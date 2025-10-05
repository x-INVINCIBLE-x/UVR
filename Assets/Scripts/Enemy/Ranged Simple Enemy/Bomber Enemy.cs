using UnityEngine;
using UnityEngine.AI;

public class BomberEnemy : SimpleEnemyBase
{
    public float selfDestructTime = 1f;
    private bool shouldGetReward = true;

    protected override void Update()
    {
        base.Update();

        if (isDead) return;

        if (!playerInSightRange && !playerInAttackRange)
            Patrol();
        else if (playerInSightRange && playerInAttackRange)
            SelfDestruct();
        else if (playerInSightRange)
            Chase();
    }

    protected override void HandleDeath()
    {
        isDead = true;
        enemyEventManager.EnemyDeath(enemyID);


        deathParticleVfx.SetActive(true);
        sfxSource.PlayOneShot(enemyDeath);

        if (agent.enabled)
            agent.SetDestination(transform.position);

        dissolver.StartDissolve();
        if (currentCheckRoutine != null)
        {
            StopCoroutine(currentCheckRoutine);
            currentCheckRoutine = null;
        }

        if (shouldGetReward)
        {
            CurrencyUI uiInstance = Instantiate(currencyUI, currencyUIOffset.position, Quaternion.identity);
            uiInstance.UpdateUI(eliminationReward.Gold, eliminationReward.Magika);
            Destroy(uiInstance.gameObject, 2f);

            GameEvents.OnElimination?.Invoke(objectiveType);
            GameEvents.RaiseReward(this);
        }

        if (ObjectPool.instance != null)
            ObjectPool.instance.ReturnObject(gameObject, 2f);
    }

    private void SelfDestruct()
    {
        shouldGetReward = false;
        Invoke(nameof(SelfKill),0.1f);
        FXManager.SelfDestructingVFX(1f);
        ObjectPool.instance.ReturnObject(gameObject,selfDestructTime);
    }

    private void SelfKill() => enemyStats.KillCharacter();
}
