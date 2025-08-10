using System.Collections;
using UnityEngine;

public class HealingTurret : Turret
{
    [SerializeField] private int healingAmount = 50;
    [SerializeField] private float healingFrequency = 0.5f;
    private WaitForSeconds healingTimer;
    private bool isHealing = false;

    private void Start()
    {
        healingTimer = new WaitForSeconds(healingFrequency);
    }

    protected override void Activate(Collider activatingCollider)
    {
        isHealing = true;

        CharacterStats stats = PlayerManager.instance.Player.Stats;
        StartCoroutine(HealingRoutine(stats));
    }

    protected override void Deactivate(Collider deactivatingCollider)
    {
        isHealing = false;
    }

    private IEnumerator HealingRoutine(CharacterStats stats)
    {
        while (isHealing)
        {
            stats.Heal(healingAmount);
            yield return healingTimer;
        }
    }
}
