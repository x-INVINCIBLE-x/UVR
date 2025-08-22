using System.Collections;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private PlayerStats playerStats;
    private bool isActive = false;
    private Coroutine damageCoroutine = null;
    private AttackData damageData;

    private void Start()
    {
        playerStats = PlayerManager.instance.Player.Stats;
    }

    public void Init(AttackData damage)
    {
        isActive = true;
        damageData = damage;
    }

    public void SetActive(bool status) => isActive = status;

    private void OnTriggerEnter(Collider other)
    {
        if (damageCoroutine != null && other.CompareTag("Player"))
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActive)
            return;

        if (damageCoroutine == null && other.CompareTag("Player"))
        {
            StartCoroutine(DamageRoutine());
        }
    }

    private IEnumerator DamageRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            playerStats.TakeDamage(damageData);
        }
    }
}
