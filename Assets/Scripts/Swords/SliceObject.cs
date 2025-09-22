using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using System.Reflection;

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
    [SerializeField] private string AfterSliceLayer = "Sliced";// sliced layer for dissolving
    void FixedUpdate()
    {
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
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
       

        if (hull != null)
        {
            int targetLayerInt = LayerMask.NameToLayer(AfterSliceLayer); // Layer set up
            // Upper hull
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            SetupSlicedComponent(upperHull, target);
            if (sliceMode == SliceMode.Single) upperHull.layer = targetLayerInt;
            if (sliceMode == SliceMode.Multi) upperHull.layer = target.layer;

            // Lower hull
            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            SetupSlicedComponent(lowerHull, target);
            if (sliceMode == SliceMode.Single) lowerHull.layer = targetLayerInt;
            if (sliceMode == SliceMode.Multi) lowerHull.layer = target.layer;

            Destroy(target);
        }

        if (target.TryGetComponent(out ISliceable sliceEvent))
        {
            sliceEvent.HandleSlice();
        }
    }

    /// <summary>
    /// Sets up Rigidbody, Collider, and copies over all scripts/components.
    /// </summary>
    public void SetupSlicedComponent(GameObject slicedObject, GameObject originalObject)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;

        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);

        // Copy over all scripts/components from the original
        CopyAllComponents(originalObject, slicedObject);
    }

    /// <summary>
    /// Copies all components (scripts, ObjectDissolver, etc.) from source to destination.
    /// </summary>
    void CopyAllComponents(GameObject source, GameObject destination)
    {
        foreach (var sourceComp in source.GetComponents<Component>())
        {
            if (sourceComp is Transform) continue;
            if (sourceComp is Rigidbody) continue;
            if (sourceComp is Collider) continue;

            System.Type type = sourceComp.GetType();

            // Add same type of component to destination
            Component destComp = destination.AddComponent(type);

            // Copy fields
            CopyFields(sourceComp, destComp);
        }
    }

    /// <summary>
    /// Copies field values from source component to destination component.
    /// Special handling for Material to make per-hull copies.
    /// </summary>
    void CopyFields(Component source, Component destination)
    {
        System.Type type = source.GetType();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (field.IsStatic) continue;

            object value = field.GetValue(source);

            // If the field is a Material, make a unique instance
            if (value is Material mat)
            {
                value = new Material(mat);
            }

            field.SetValue(destination, value);
        }
    }
}
