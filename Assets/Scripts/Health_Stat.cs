using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Stat : MonoBehaviour, IDamageable
{
    [SerializeField] private ObjectiveType objectiveType = ObjectiveType.Turret;
    [SerializeField] private MeshDissolver dissolver;
    public int maxHealth;
    public int currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public DamageResult TakeDamage(AttackData attackData)
    {
        currentHealth -= Mathf.Max(0,(int)attackData.physicalDamage.Value);

        if (currentHealth <= 0)
        {
            Eliminate();
        }

        dissolver.StartImpactDissolve(0.1f);
        return new DamageResult(true, currentHealth <= 0, attackData.physicalDamage.Value, null);
    }

    private void Eliminate()
    {   
        dissolver.StartDissolve();
        GameEvents.OnElimination?.Invoke(objectiveType);
        Destroy(gameObject);
    }

    public void Heal(float amount)
    {

    }
}
