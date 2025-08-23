using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    DamageResult TakeDamage(AttackData attackData);

    void Heal(float amount);
}
