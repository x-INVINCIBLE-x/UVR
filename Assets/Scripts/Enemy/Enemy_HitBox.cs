using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HitBox : HitBox
{
    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponentInParent<Enemy>();
    }

    public override void TakeDamage(AttackData damage)
    {
        AttackData _damage = Instantiate(damage);

        _damage.physicalDamage.BaseValue *= damageMultiplier;

        enemy.GetHit(_damage);
    }
}
