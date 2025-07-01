using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Rifle : RangedWeapon
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int clickCount;



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
        Debug.Log("rbt");
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




        // Instantiate the projectile
        //PhysicsProjectile projectileInstance = Instantiate(projectilePrefab, spawnPoints[clickCount].position, spawnPoints[clickCount].rotation) as PhysicsProjectile; //.GetComponent<PhysicsProjectile>();
        //projectileInstance.Init();
        


        GameObject newProjectile = ObjectPool.instance.GetObject(projectilePrefab.gameObject, spawnPoints[clickCount]);
        PhysicsProjectile projectileInstance = newProjectile.GetComponent<PhysicsProjectile>();
        projectileInstance.Init();
        projectileInstance.Launch(spawnPoints[clickCount], shootingForce);

        clickCount = (clickCount + 1) % spawnPoints.Length; // cyclic buffer , if we put mode(%) in equation it cannot exceed the value which is modding it.

        // SFX Implement
        WeaponAudioSource.PlayOneShot(shootSFX);

        // VFX Implement
        GameObject newMuzzleVFX = ObjectPool.instance.GetObject(muzzleVFX, spawnPoints[clickCount]);
        newMuzzleVFX.transform.rotation = spawnPoints[clickCount].rotation;
        ObjectPool.instance.ReturnObject(newMuzzleVFX, 1f);

    }

    // Implement cool down here




}
