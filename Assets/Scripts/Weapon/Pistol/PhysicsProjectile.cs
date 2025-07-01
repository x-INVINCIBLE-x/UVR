using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class PhysicsProjectile : Projectile
{
    [SerializeField] private float lifeTime;
    public GameObject impactVFX;
    private GameObject currentImpactVFX;
    private Rigidbody rigidBody;
    private AudioSource audioSource;
    public AudioClip impactSFX;
    private ParticleSystem VFXparticleSystem;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
    }
    private void Start()
    {
        VFXparticleSystem = GetComponent<ParticleSystem>();
    }

    public override void Init()
    {
        base.Init();
        //Destroy(gameObject , lifeTime);
        ObjectPool.instance.ReturnObject(gameObject, lifeTime);
    }

    public override void Launch(Transform _transform, float force)
    {
        base.Launch(_transform, force);
        rigidBody.linearVelocity = _transform.forward * force;
        transform.rotation = Quaternion.LookRotation(rigidBody.linearVelocity.normalized);
    }

    private void OnTriggerEnter(Collider other)
    {

        ObjectPool.instance.ReturnObject(gameObject, 1f);

        
        if (VFXparticleSystem != null)
        {
            VFXparticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (impactVFX != null)
        {
            currentImpactVFX = ObjectPool.instance.GetObject(impactVFX.gameObject, transform);
            ObjectPool.instance.ReturnObject(currentImpactVFX, 0.5f);
        }

        if (impactSFX != null)
        {
            audioSource.PlayOneShot(impactSFX);
        }

    }
    
}
