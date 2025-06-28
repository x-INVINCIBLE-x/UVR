using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public Transform playerBody { get; internal set; }
    public PlayerStats stats { get; private set; }

    private void Awake()
    {
        stats = GetComponentInChildren<PlayerStats>();
    }
}