using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HealingTurret : Turret
{
    [Header("Healing Properties")]
    [SerializeField] private int healingAmount = 50;
    [SerializeField] private float healingFrequency = 0.5f;
    private WaitForSeconds healingTimer;
    private Coroutine healingCoroutine;
    private bool isHealing = false;

    [Header("Healing Limiter")]
    [SerializeField] private bool hasLimit = false;
    [SerializeField] private int healingLimit = 100;
    private int healed = 0;

    private void Start()
    {
        healingTimer = new WaitForSeconds(healingFrequency);
    }

    protected override void Activate(Collider activatingCollider)
    {
        isHealing = true;

        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
        }

        CharacterStats stats = PlayerManager.instance.Player.Stats;
        healingCoroutine = StartCoroutine(HealingRoutine(stats));
    }

    protected override void Deactivate(Collider deactivatingCollider)
    {
        isHealing = false;
    }

    private void OnHealComplete()
    {
        Debug.Log("Heal Complete");
    }

    private IEnumerator HealingRoutine(CharacterStats stats)
    {
        while (isHealing)
        {
            if (hasLimit && healed >= healingLimit)
            {
                OnHealComplete();
                yield break;
            }

            stats.Heal(healingAmount);
            healed += healingAmount;

            yield return healingTimer;
        }

        healingCoroutine = null;
    }
}
