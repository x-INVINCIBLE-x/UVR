using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class Turret : MonoBehaviour, ISliceable
{
    protected string activationTag = "Player";
    protected SphereCollider col;
    private bool isActivated = true;
    protected bool isSliced = false;

    protected virtual void Awake()
    {
        col = GetComponent<SphereCollider>();
    }

    protected abstract void Activate();

    protected abstract void Deactivate();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (isSliced) return;

        if (other.CompareTag(activationTag))
        {
            isActivated = true;
            Activate();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (isActivated == true && other.CompareTag(activationTag))
        {
            isActivated = false;
            Deactivate();
        }
    }

    public virtual void HandleSlice()
    {
        isSliced = true;
        col.enabled = false;
        Deactivate();
    }
}
