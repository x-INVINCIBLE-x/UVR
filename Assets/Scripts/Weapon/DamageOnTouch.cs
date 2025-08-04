using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [Tooltip("Either Set up by other Script or give its own AttackData. If both then combines the AttackData")]
    [SerializeField] private AttackData attackData;
    [SerializeField] private float damageRate = 0.2f;
    private readonly HashSet<IDamageable> damagables = new();
    private float timer;
    [SerializeField] private LayerMask layerToDamage;

    private void Update()
    {
        timer -= Time.deltaTime; 
        if (timer < 0)
        {
            timer = damageRate;
            foreach (IDamageable damagable in damagables)
            {
                damagable.TakeDamage(attackData);
            }
        }
    }

    public void Setup(AttackData attackData, float damageRate, LayerMask mask)
    {
        this.attackData = attackData;
        this.damageRate = damageRate;
        layerToDamage = mask;
        timer = damageRate;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerToDamage) == 0) return;

        IDamageable damagable = other.GetComponentInParent<IDamageable>();
        damagable ??= other.GetComponentInChildren<IDamageable>();
        if (damagable != null)
        {
            Debug.Log("Give damage");
            damagable.TakeDamage(attackData);
            damagables.Add(damagable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable damagable = other.GetComponent<IDamageable>();
        if (damagables.Contains(damagable))
        {
            damagables.Remove(damagable);
        }
    }

    private void OnDisable()
    {
        damagables.Clear();
    }
}
