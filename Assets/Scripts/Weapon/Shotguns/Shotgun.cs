using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class Shotgun : ScatterWeapons
{
    [SerializeField] private Projectile projectilePrefab;


    protected override void Awake()
    {
        base.Awake();
        
    }
    

    protected override void ScatterShot()
    {
        base.ScatterShot();
        
        foreach (Transform spawnPoint in bulletSpawns)
        {
            PhysicsProjectile projectileInstance = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation) as PhysicsProjectile;
            projectileInstance.Init();
            Rigidbody rb = projectileInstance.GetComponent<Rigidbody>(); // Takes rigidbody of each bullet

            // Write logic for scattering bullets

            if (rb != null )
            {
                Vector3 spreadDirection = Quaternion.Euler(
                    Random.Range(-spreadAngle, spreadAngle), // Random X rotation (up/down) 
                    Random.Range(-spreadAngle, spreadAngle), // Random Y rotation (left/right)
                    0 // No roll rotation
                ) * spawnPoint.forward;

                rb.linearVelocity = spreadDirection * shootingForce;
            }

            //Error because rb.linearVelocity = spreadDirection * shootingForce; is not applied so no spread
            //projectileInstance.Launch(spawnPoint, shootingForce);
        }


    }
}
