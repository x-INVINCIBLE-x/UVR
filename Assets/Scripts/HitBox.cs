using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour, IDamageable
{
    [SerializeField] protected float damageMultiplier = 1f;

    protected virtual void Awake()
    {

    }

    public virtual DamageResult TakeDamage(AttackData attackData)
    {
        return null;
    }

    public void Heal(float amount)
    {
    }
}
