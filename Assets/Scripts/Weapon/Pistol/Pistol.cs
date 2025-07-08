using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Pistol : RangedWeapon
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float minPitch = 1f;
    [SerializeField] protected float maxPitch = 3f;

    protected override void Awake()
    {
        base.Awake();

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    protected override void ActivateWeapon(ActivateEventArgs args)
    {
        base.ActivateWeapon(args);
        Shoot();

    }

    protected override void DeactivateWeapon(DeactivateEventArgs args)
    {
        base.DeactivateWeapon(args);

    }

    protected override void Shoot()
    {
        base.Shoot();

        GameObject newProjectile = ObjectPool.instance.GetObject(projectilePrefab.gameObject, bulletSpawn);
        PhysicsProjectile projectileInstance = newProjectile.GetComponent<PhysicsProjectile>();
        projectileInstance.Init();

        projectileInstance.Launch(bulletSpawn, shootingForce);

        // VFX Implement
        GameObject newMuzzleVFX = ObjectPool.instance.GetObject(muzzleVFX, bulletSpawn);
        newMuzzleVFX.transform.rotation = bulletSpawn.rotation;
        ObjectPool.instance.ReturnObject(newMuzzleVFX, 1f);

        // SFX Implement
        ShootAudio();

    }

    private void ShootAudio()
    {
        WeaponAudioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        WeaponAudioSource.PlayOneShot(shootSFX);
    }

}
