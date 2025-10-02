using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnBoxCheck : MonoBehaviour
{
    [Header("Box Check Settings")]
    [Tooltip("Local center of the box relative to this transform.")]
    [SerializeField] private Vector3 boxCenter = Vector3.zero;

    [Tooltip("Size of the box (full extents).")]
    [SerializeField] private Vector3 boxSize = new Vector3(1f, 1f, 1f);

    [Tooltip("Local rotation offset applied to the box (degrees).")]
    [SerializeField] private Vector3 boxRotationOffset = Vector3.zero;

    [Tooltip("How often (seconds) the manual check runs. 0 = every FixedUpdate.")]
    [SerializeField] private float checkInterval = 0.1f;

    [Tooltip("Whether to destroy this object too when a target is found.")]
    [SerializeField] private bool destroySelf = false;

    [Tooltip("Should triggers be included in the check?")]
    [SerializeField] private QueryTriggerInteraction queryTriggers = QueryTriggerInteraction.Ignore;

    [Header("Filter")]
    [Tooltip("If true, uses name contains 'block' to decide destruction. If false, destroys all hits.")]
    [SerializeField] private bool requireNameContainsBlock = true;
    [SerializeField] private LayerMask destroyLayers = ~0;

    [Header("Gizmo Appearance")]
    [SerializeField] private Color gizmoFillColor = new Color(1f, 0f, 0f, 0.12f);
    [SerializeField] private Color gizmoWireColor = new Color(1f, 0f, 0f, 1f);

    // internal
    private float checkTimer = 0f;
    private Coroutine checkRoutine = null;
    private float limit = 3f;
    private float currLimit = 3f;
    // Keep a set of roots we've already destroyed this run to avoid double-destroy attempts
    private HashSet<GameObject> destroyedRoots = new HashSet<GameObject>();

    private void Reset()
    {
        // sensible defaults
        boxCenter = Vector3.forward * 1f;
        boxSize = new Vector3(1f, 1f, 1f);
        checkInterval = 0.1f;
        gizmoFillColor = new Color(1f, 0f, 0f, 0.12f);
        gizmoWireColor = new Color(1f, 0f, 0f, 1f);
    }

    private void OnEnable()
    {
        DoBoxCheck();
        currLimit = limit;
        // Start the repeated check coroutine
        if (checkRoutine != null)
            StopCoroutine(checkRoutine);

        checkRoutine = StartCoroutine(BoxCheckCoroutine());
    }

    private void OnDisable()
    {
        if (checkRoutine != null)
        {
            StopCoroutine(checkRoutine);
            checkRoutine = null;
        }
    }

    private IEnumerator BoxCheckCoroutine()
    {
        // Use default interval if checkInterval <= 0
        float interval = checkInterval > 0f ? checkInterval : 0.5f;

        while (currLimit >= 0)
        {
            yield return new WaitForSeconds(interval);
            DoBoxCheck();
            currLimit -= interval;
        }
    }


    private void DoBoxCheck()
    {
        Collider[] hits = Physics.OverlapBox(
            transform.position,
            boxSize * 0.5f,
            transform.rotation,
            destroyLayers
        );

        foreach (Collider hit in hits)
        {
            if (hit == null) continue;

            Debug.Log($"Hit collider: {hit.name}");

            Transform current = hit.transform;
            int depth = 0;
            int maxDepth = 50;

            // Walk up the hierarchy, checking each step
            while (current != null && depth < maxDepth)
            {
                Debug.Log($"Step {depth}: {current.name}");

                GameObject obj = current.gameObject;

                // Skip if it's our own root
                if (obj == this.transform.root.gameObject || obj.transform.root == this.transform)
                {
                    Debug.Log("Skipping self root");
                }
                else
                {
                    // Check if this step's object name contains "block"
                    if (obj.name.IndexOf("block", System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Debug.Log($"Destroying object at step {depth}: {obj.name}");
                        Destroy(obj);
                        break; // stop after destroying
                    }
                }

                current = current.parent;
                depth++;
            }
        }
    }


    #region Gizmos
    private void OnDrawGizmos()
    {
        DrawBoxGizmos(false);
    }

    private void OnDrawGizmosSelected()
    {
        DrawBoxGizmos(true);
    }

    private void DrawBoxGizmos(bool selected)
    {
        // compute transform
        Vector3 worldCenter = transform.TransformPoint(boxCenter);
        Quaternion worldRot = transform.rotation * Quaternion.Euler(boxRotationOffset);
        Vector3 halfExtents = boxSize * 0.5f;

        // Save matrix so we can draw in box-local space
        Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(worldCenter, worldRot, Vector3.one);

        // Fill (semi-transparent)
        Color prevColor = Gizmos.color;
        if (selected)
        {
            Gizmos.color = gizmoFillColor;
            // Draw a solid cube by drawing many wire cubes is expensive; use a proxy: draw cube via a cube mesh
            // Note: Gizmos.DrawCube is fine for a filled-looking cube
            Gizmos.DrawCube(Vector3.zero, boxSize);
        }

        // Wireframe
        Gizmos.color = gizmoWireColor;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);

        Gizmos.color = prevColor;
        Gizmos.matrix = oldGizmosMatrix;
    }
    #endregion
}
