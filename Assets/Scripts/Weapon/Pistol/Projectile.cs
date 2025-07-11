using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
   //protected Weapon weapon;

    public virtual void Init(float _lifeTime, AttackData _attackData)
    {
    }

    public virtual void Launch(Transform _transform, float shootingForce, Vector3? dir = null)
    {

    }
}
