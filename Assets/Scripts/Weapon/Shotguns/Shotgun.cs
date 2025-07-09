using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Shotgun : ScatterWeapons
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private float minPitch = 1f;
    [SerializeField] protected float maxPitch = 3f;
    [SerializeField] protected float bulletLifeTime = 5f;

    protected override void Awake()
    {
        base.Awake();

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScatterShot();
        }
    }

    protected override void ActivateWeapon(ActivateEventArgs args)
    {
        Debug.Log("rbt");
        base.ActivateWeapon(args);
        ScatterShot();

    }

    protected override void DeactivateWeapon(DeactivateEventArgs args)
    {
        base.DeactivateWeapon(args);

    }

    protected override void ScatterShot()
    {
        base.ScatterShot();

        // SFX Implement
        ShootAudio();

        // VFX Implement
        GameObject newMuzzleVFX = ObjectPool.instance.GetObject(muzzleVFX, bulletSpawns[0]);// hot fix muzzle is spawned at the position of 0 index bullet spawn
        newMuzzleVFX.transform.rotation = bulletSpawns[0].rotation;
        ObjectPool.instance.ReturnObject(newMuzzleVFX, 1f);


        foreach (Transform spawnPoint in bulletSpawns)
        {
            //PhysicsProjectile projectileInstance = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation) as PhysicsProjectile;

            GameObject newProjectile = ObjectPool.instance.GetObject(projectilePrefab.gameObject, spawnPoint);
            PhysicsProjectile projectileInstance = newProjectile.GetComponent<PhysicsProjectile>();
            projectileInstance.Init(bulletLifeTime, finalAttackData);


            Rigidbody rb = projectileInstance.GetComponent<Rigidbody>(); // Takes rigidbody of current instanced bullet


            // Write logic for scattering bullets

            if (rb != null)
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
    private void ShootAudio()
    {
        WeaponAudioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        WeaponAudioSource.PlayOneShot(shootSFX);
    }
}
