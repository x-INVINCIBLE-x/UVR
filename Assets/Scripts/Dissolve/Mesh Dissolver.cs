using System;
using System.Collections;
using UnityEngine;
public class MeshDissolver : MonoBehaviour
{
    [Header("References")]
    public MeshRenderer mesh;
    public Material DissolveMaterial;
    public bool Dissolve;
    private bool isDissolving;

    [SerializeField] private Material[] defaultMaterials;
    [SerializeField] private Material[] meshMaterials;


    [SerializeField] private float dissolveRate = 0.0125f;
    [SerializeField] private float RefreshRate = 0.025f;

    private static readonly int DissolveAmountID = Shader.PropertyToID("_Dissolve_Amount");

    private void Awake()
    {
        if (DissolveMaterial == null)
        {
            Debug.Log("Dissolve Material not Assigned");
        }

        if (mesh != null)
        {
            defaultMaterials = mesh.sharedMaterials;
            meshMaterials = new Material [defaultMaterials.Length];


            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                meshMaterials[i] = new Material(DissolveMaterial);
            }
        }

        


    }

    private void Update()
    {

        // to remove this update statements(important)
        if (Dissolve)
        {
            StartDissolver();
        }
        else
        {
            ResetDissolver();
        }
    }

    private void StartDissolver()
    {
        if (!isDissolving)
        {
            isDissolving = true;
            mesh.materials = meshMaterials;
            StartCoroutine(DissolveMesh());
        }


    }

    private IEnumerator DissolveMesh()
    {
        if (meshMaterials.Length > 0)
        {
            float currentDissolve = 0;

            while (currentDissolve < 1f)
            {
                currentDissolve += dissolveRate;
                currentDissolve = Mathf.Clamp01(currentDissolve);

                for (int i = 0; i < meshMaterials.Length; i++)
                {
                    meshMaterials[i].SetFloat(DissolveAmountID, currentDissolve);
                }

                yield return new WaitForSeconds(RefreshRate);
            }

        }


    }

    public void ActivateSkinnedDissolver(bool dissolveStatus)
    {
        Dissolve = dissolveStatus;

        if (dissolveStatus)
        {
            StartDissolver();
        }
    }
    public void ResetDissolver()
    {
        StopAllCoroutines();
        isDissolving = false;

        // Reset dissolve amount on all instances
        foreach (var material in meshMaterials)
        {
            material.SetFloat(DissolveAmountID, 0f);
        }

        // Restore original materials
        mesh.materials = defaultMaterials;
    }

    private void OnDisable()
    {
        ResetDissolver();
    }

    private void OnDestroy()
    {

        if (meshMaterials != null)
        {
            foreach (var material in meshMaterials)
            {
                if (material != null)
                {
                    Destroy(material);
                }
            }
        }
    }
}
