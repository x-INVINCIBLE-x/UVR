using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class PhysicsProjectile : Projectile
{
    [SerializeField] private float lifeTime;
    private AttackData attackData;

    public GameObject impactVFX;
    private GameObject currentImpactVFX;
    private Rigidbody rigidBody;
    private AudioSource audioSource;
    public AudioClip impactSFX;
    private ParticleSystem VFXparticleSystem;

    private HashSet<IDamagable> damaged = new();

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
    }

    private void OnEnable()
    {
        damaged.Clear();
    }

    private void Start()
    {
        VFXparticleSystem = GetComponent<ParticleSystem>();
    }

    public override void Init(float _lifeTime, AttackData _attackData)
    {
        base.Init(_lifeTime, _attackData);
        lifeTime = _lifeTime;
        attackData = _attackData;
        ObjectPool.instance.ReturnObject(gameObject, lifeTime);
    }

    public override void Launch(Transform _transform, float force, Vector3? dir = null)
    {
        base.Launch(_transform, force);
        Vector3 lookDirection = dir ?? _transform.forward;
        rigidBody.linearVelocity = lookDirection * force;
        transform.rotation = Quaternion.LookRotation(rigidBody.linearVelocity.normalized);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamagable damagable = other.GetComponentInParent<IDamagable>();
        damagable ??= other.GetComponentInChildren<IDamagable>();
        if (damagable != null && !damaged.Contains(damagable) && attackData != null)
        {
            damagable.TakeDamage(attackData);
            damaged.Add(damagable);
        }
        
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
