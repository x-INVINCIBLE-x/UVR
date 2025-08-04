using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    void TakeDamage(AttackData attackData);

    void Heal(float amount);
}
