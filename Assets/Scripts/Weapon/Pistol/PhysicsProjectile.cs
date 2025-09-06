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
    public float impactDuration = 0.5f;
    private ParticleSystem VFXparticleSystem;

    private HashSet<IDamageable> damaged = new();
    private Collider col;

    [SerializeField] private float blastRadius = 0f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        damaged.Clear();
        if (col != null)
            col.enabled = true;
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
        col.enabled = true;
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
        IDamageable damagable = other.GetComponentInParent<IDamageable>();
        damagable ??= other.GetComponentInChildren<IDamageable>();
        if (damagable != null && !damaged.Contains(damagable) && attackData != null)
        {
            DamageResult result = damagable.TakeDamage(attackData);
            if (attackData.owner != null && result != null)
            {
                attackData.owner.RaiseOnDamageGiven(result);
            }

            damaged.Add(damagable);
        }

        col.enabled = false;
        ObjectPool.instance.ReturnObject(gameObject, 1f);
        rigidBody.linearVelocity = Vector3.zero;

        if (blastRadius > 0f)
        {
            Collider[] colliders = new Collider[20];
            int count = Physics.OverlapSphereNonAlloc(transform.position, blastRadius, colliders);
            for (int i = 0; i < count; i++)
            {
                Collider collider = colliders[i];
                if (collider == null) break;
                IDamageable blastDamagable = collider.GetComponentInParent<IDamageable>();
                blastDamagable ??= collider.GetComponentInChildren<IDamageable>();
                if (blastDamagable != null && !damaged.Contains(blastDamagable) && attackData != null)
                {
                    DamageResult result = blastDamagable.TakeDamage(attackData);
                    if (attackData.owner != null && result != null)
                    {
                        attackData.owner.RaiseOnDamageGiven(result);
                    }
                    damaged.Add(blastDamagable);
                }
            }
        }

        if (VFXparticleSystem != null)
        {
            VFXparticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (impactVFX != null)
        {
            currentImpactVFX = ObjectPool.instance.GetObject(impactVFX, transform.position);
            ObjectPool.instance.ReturnObject(currentImpactVFX, impactDuration);
        }

        if (impactSFX != null)
        {
            audioSource.PlayOneShot(impactSFX);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (blastRadius > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, blastRadius);
        }
    }
}
