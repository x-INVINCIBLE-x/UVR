using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;

public class SliceObject : MonoBehaviour
{
    public enum SliceMode
    {
        Single,
        Multi
    }
    public SliceMode sliceMode;

    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public LayerMask sliceableLayer;
    public VelocityEstimator velocityEstimator;

    public Material crossSectionMaterial;
    public float cutForce = 2000f;

    void FixedUpdate()
    {
        bool hasHit  = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        if (hasHit)
        {
            GameObject target = hit.transform.gameObject;
            Slice(target);
        }
    }

    public void Slice(GameObject target)
    {
        target.transform.parent = null;
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();
        
        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        // Check for Slice Mode

        // If the slice mode is multi then allows for multiple cuts
        // upper anf lower hull object's will have the sliceable layer
        if(sliceMode == SliceMode.Multi)
        {
            if (hull != null)
            {
                GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
                SetupSlicedComponent(upperHull);
                upperHull.layer = target.layer;

                GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
                SetupSlicedComponent(lowerHull);
                lowerHull.layer = target.layer;

                Destroy(target);
            }
        }

        // if slice mode is single , then only alloes the object to be cut once
        // upper anf lower hull object's will have not have the sliceable layer
        if (sliceMode == SliceMode.Single)
        {
            if (hull != null)
            {
                GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
                SetupSlicedComponent(upperHull);
                

                GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
                SetupSlicedComponent(lowerHull);
                

                Destroy(target);
            }
        }

        // Implement Slice Event

        if(target.TryGetComponent(out ISliceable sliceEvent))
        {
            sliceEvent.HandleSlice();
        }
        
    }

    /// <summary>
    /// This function setups the new hulled object's properties
    /// Can be used to implement things that happen after an object is sliced
    /// </summary>
    /// <param name="slicedObject"></param>
    public void SetupSlicedComponent(GameObject slicedObject)
    {   

        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;

        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);        
    }
}