using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Pistol : RangedWeapon
{
    [SerializeField] private Projectile projectilePrefab;
    protected override void ActivateWeapon(ActivateEventArgs args)
    {
        base.ActivateWeapon(args);
        Shoot();
    }

    protected override void Shoot()
    {
        base.Shoot();
        Projectile projectileInstance = Instantiate(projectilePrefab , bulletSpawn.position , projectilePrefab.transform.rotation);
        projectileInstance.Init();
        projectileInstance.Launch(bulletSpawn, shootingForce);
    }

    protected override void DeactivateWeapon(DeactivateEventArgs args)
    {
        base.DeactivateWeapon(args);
    }
}
