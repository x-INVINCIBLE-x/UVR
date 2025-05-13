using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    [field: SerializeField] public Transform playerBody { get; internal set; }
    public PlayerStats stats { get; private set; }

    public event System.Action OnDamageTaken;

    private void Awake()
    {
        stats = GetComponentInChildren<PlayerStats>();
    }

    public void TakeDamage(AttackData attackData)
    {
        stats.TakeDamage(attackData);
        OnDamageTaken.Invoke();
    }
}