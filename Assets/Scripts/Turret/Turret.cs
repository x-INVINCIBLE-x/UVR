using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class Turret : MonoBehaviour, ISliceable
{
    protected string activationTag = "Player";
    protected SphereCollider col;
    protected bool isActive = true;
    protected bool isSliced = false;
    private float bufferTime = 0.1f;
    private float lastActivationTime = -1f;

    protected virtual void Awake()
    {
        col = GetComponent<SphereCollider>();
    }

    protected abstract void Activate(Collider activatingCollider);

    protected abstract void Deactivate(Collider deactivatingCollider);

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (isSliced) return;

        if (other.CompareTag(activationTag) && lastActivationTime + bufferTime < Time.time)
        {
            isActive = true;
            lastActivationTime = Time.time;
            Activate(other);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (isActive == true && other.CompareTag(activationTag))
        {
            isActive = false;
            Deactivate(other);
        }
    }

    public virtual void HandleSlice()
    {
        isActive = false;
        isSliced = true;
        col.enabled = false;
        Deactivate(null);
    }

    public void HandleInstanceSlice()
    {
    }

    public void PreSlice()
    {
    }
}
