using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [Tooltip("Either Set up by other Script or give its own AttackData. If both then combines the AttackData")]
    private AttackData attackData;
    private float damageRate = 0.2f;
    private readonly HashSet<IDamagable> damagables = new();
    private float timer;
    private bool damageOnlyPlayer = true;
    private LayerMask playerLayer;

    private void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
    }

    private void Update()
    {
        timer -= Time.deltaTime; 
        if (timer < 0)
        {
            timer = damageRate;
            foreach (IDamagable damagable in damagables)
            {
                damagable.TakeDamage(attackData);
            }
        }
    }

    public void Setup(AttackData attackData, float damageRate, bool damageOnlyPlayer = true)
    {
        this.attackData = attackData;
        this.damageRate = damageRate;
        this.damageOnlyPlayer = damageOnlyPlayer;
        timer = damageRate;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (damageOnlyPlayer && ((1 << other.gameObject.layer) & playerLayer) == 0) return;

        IDamagable damagable = other.GetComponentInParent<IDamagable>();
        damagable ??= other.GetComponentInChildren<IDamagable>();
        if (damagable != null)
        {
            damagables.Add(damagable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if (damagables.Contains(damagable))
        {
            damagables.Remove(damagable);
        }
    }
}
