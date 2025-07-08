using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class Turret : MonoBehaviour
{
    private bool isActivated = true;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActivated = true;
            Activate();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (isActivated == true && other.CompareTag("Player"))
        {
            isActivated = false;
            Deactivate();
        }
    }

    protected abstract void Activate();

    protected abstract void Deactivate();
}
