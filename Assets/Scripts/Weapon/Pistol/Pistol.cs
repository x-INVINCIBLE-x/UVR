using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Pistol : RangedWeapon
{
    [SerializeField] private Projectile projectilePrefab;
    private float chargeTime = 0f;
    private bool isCharging = false;
    public float maxCharge = 3f;
    public float maxbulletSize = 2f;
    public float maxShootingForce = 50f;// Max spped of bullet

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        if (isCharging)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxCharge);
        }
    }


    protected override void ActivateWeapon(ActivateEventArgs args)
    {
        Debug.Log("rbt");
        base.ActivateWeapon(args);
        isCharging = true;
        chargeTime = 0f;
        //Shoot();
    }

    protected override void DeactivateWeapon(DeactivateEventArgs args)
    {
        base.DeactivateWeapon(args);
        if (isCharging) 
        {
            isCharging = false;
            Shoot();
            AudioHandlerPistol();

        }
    }

    protected override void Shoot()
    {
        base.Shoot();

        // calculate the charged ratio
        float chargeRatio = chargeTime / maxCharge;
        chargeRatio = Mathf.Clamp(chargeRatio,0f,1f); // Clamp btw 0 and 1


        // Instantiate the projectile
        //PhysicsProjectile projectileInstance = Instantiate(projectilePrefab , bulletSpawn.position , bulletSpawn.rotation)as PhysicsProjectile; //.GetComponent<PhysicsProjectile>();
        

        GameObject newProjectile = ObjectPool.instance.GetObject(projectilePrefab.gameObject, bulletSpawn);
        PhysicsProjectile projectileInstance = newProjectile.GetComponent<PhysicsProjectile>();
        projectileInstance.Init();


        // Scale the bulllet based on charged ratio
        float bulletscale = Mathf.Lerp(1f, maxbulletSize, chargeRatio);
        projectileInstance.transform.localScale *= bulletscale;

        // Increase shooting force based on charge ratio
        float appliedforce = Mathf.Lerp(shootingForce , maxShootingForce , chargeRatio);
        projectileInstance.Launch(bulletSpawn, appliedforce);

        chargeTime = 0f; // Reset after shooting
    }





    [Header(" AudioClips")]
    [SerializeField] private AudioClip _shootingSoundPistol;
    [SerializeField] private AudioClip chargingSoundPistol;

    [Header("AudioSettings")]
    [SerializeField] private float minPitch = 1f;
    [SerializeField] private float maxPitch = 2f;
    [SerializeField] private float chargedPitchMin = 0.7f;
    [SerializeField] private float chargedPitchMax = 1f;

    private AudioSource audioSource;

    private void AudioHandlerPistol()
    {
        if(isCharging == false)
        {
            if(audioSource.clip != _shootingSoundPistol || !audioSource.isPlaying)
            {   
                audioSource.Stop(); // to stop ongoing sfx sounds
                audioSource.clip = _shootingSoundPistol;
                audioSource.loop = false;
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.Play();
            }
            
        }
        if(isCharging == true)
        {
            if (chargeTime <= 0.01f)
            {
                audioSource.Stop();
            }

            else if (audioSource.clip != chargingSoundPistol || !audioSource.isPlaying)
            {
                audioSource.clip = chargingSoundPistol;
                audioSource.pitch = Mathf.Lerp(chargedPitchMin, chargedPitchMax, chargeTime);
                audioSource.loop = true;
                audioSource.Play();  
            }
            
            

        }

    }
    



}
