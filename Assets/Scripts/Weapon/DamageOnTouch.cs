using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [Tooltip("Either Set up by other Script or give its own AttackData. If both then combines the AttackData")]
    [SerializeField] private AttackData attackData;
    [SerializeField] private float damageRate = 0.2f;
    [SerializeField] private float bufferDuration = 1f; 
    [SerializeField] private LayerMask layerToDamage;

    private readonly HashSet<IDamageable> damagables = new();
    private readonly HashSet<IDamageable> buffer = new();
    private float timer;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
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

        if (damagable != null && !damagables.Contains(damagable) && !buffer.Contains(damagable))
        {
            Debug.Log("Give damage");
            damagable.TakeDamage(attackData);
            damagables.Add(damagable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable damagable = other.GetComponentInParent<IDamageable>()
                          ?? other.GetComponentInChildren<IDamageable>();

        if (damagable != null && damagables.Contains(damagable))
        {
            damagables.Remove(damagable);
            buffer.Add(damagable);
            StartCoroutine(RemoveFromBufferAfterDelay(damagable, bufferDuration));
        }
    }

    private IEnumerator RemoveFromBufferAfterDelay(IDamageable damagable, float delay)
    {
        yield return new WaitForSeconds(delay);
        buffer.Remove(damagable);
    }

    private void OnDisable()
    {
        damagables.Clear();
        buffer.Clear();
    }
}
