using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PhysicsProjectile : Projectile
{
    [SerializeField] private float lifeTime;
    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public override void Init()
    {
        base.Init(); 
        Destroy(gameObject , lifeTime);

    }

    public override void Launch(Transform _transform, float force)
    {
        base.Launch(_transform, force);
        rigidBody.linearVelocity = _transform.forward * force;
    }
}
