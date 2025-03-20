using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
   protected Weapon weapon;

    public virtual void Init()
    {
    }

    public virtual void Launch(Transform _transform, float shootingForce)
    {

    }
}
