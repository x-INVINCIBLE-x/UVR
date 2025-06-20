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
            //PhysicsProjectile projectileInstance = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation) as PhysicsProjectile;

            GameObject newProjectile = ObjectPool.instance.GetObject(projectilePrefab.gameObject, spawnPoint);
            PhysicsProjectile projectileInstance = newProjectile.GetComponent<PhysicsProjectile>();
            projectileInstance.Init();


            Rigidbody rb = projectileInstance.GetComponent<Rigidbody>(); // Takes rigidbody of current instanced bullet
            

            // Write logic for scattering bullets

            if (rb != null )
            {
                Vector3 spreadDirection = Quaternion.Euler(
                    Random.Range(-spreadAngle, spreadAngle), // Random X rotation (up/down) 
                    Random.Range(-spreadAngle, spreadAngle), // Random Y rotation (left/right)
                    0 // No roll rotation
                ) * spawnPoint.forward;

                rb.linearVelocity = spreadDirection * shootingForce;
                rb.transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);// makes the bullet go straight 
            }

            //Error because rb.linearVelocity = spreadDirection * shootingForce; is not applied so no spread
            //projectileInstance.Launch(spawnPoint, shootingForce);
        }


    }
}
