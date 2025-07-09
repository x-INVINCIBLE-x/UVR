using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class SprayingWeapons : Weapon
{

    protected AudioSource WeaponAudioSource;

    private void Start()
    {
        InputManager.Instance.activate.action.performed += ctx => StartSpraying();
        InputManager.Instance.activate.action.canceled += ctx => StopSpraying();
    }

    protected override void Awake()
    {
        base.Awake();
        WeaponAudioSource = GetComponent<AudioSource>();

    }

    // Flamethrower , acid thrower and other spray type weapons
    protected virtual void StartSpraying()
    {

    }

    protected virtual void StopSpraying()
    {

    }

    private void OnDisable()
    {
        InputManager.Instance.activate.action.performed -= ctx => StartSpraying();
        InputManager.Instance.activate.action.canceled -= ctx => StopSpraying();
    }
}
