using System;
using System.Collections;
using UnityEngine;

public class SkinnedMeshDissolver : MonoBehaviour
{
    [Header("References")]
    public SkinnedMeshRenderer skinnedMesh;
    public Material DissolveMaterial;
    public bool Dissolve;
    private bool isDissolving;

    [SerializeField] private Material[] defaultMaterials;
    [SerializeField] private Material[] skinnedMaterials;

   
    [SerializeField] private float dissolveRate = 0.0125f;
    [SerializeField] private float RefreshRate = 0.025f;

    private static readonly int DissolveAmountID = Shader.PropertyToID("_Dissolve_Amount");
    private void Awake()
    {
        if (DissolveMaterial == null)
        {
            Debug.Log("Dissolve Material not Assigned");
        }

        if (skinnedMesh != null)
        {
            defaultMaterials = skinnedMesh.sharedMaterials;
            skinnedMaterials = new Material[defaultMaterials.Length];

            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                skinnedMaterials[i] = new Material(DissolveMaterial);  
            }
            //defaultMaterials = skinnedMesh.sharedMaterials; 
            //skinnedMaterials = skinnedMesh.sharedMaterials;
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

    // Public for external use
    public void ActivateSkinnedDissolver(bool dissolveStatus)
    {
        Dissolve = dissolveStatus;

        if (dissolveStatus)
        {
            StartDissolver();
        }
    }

    private void StartDissolver()
    {
        if (!isDissolving)
        {
            isDissolving = true;
            skinnedMesh.materials = skinnedMaterials;
            StartCoroutine(DissolveSkinnedMesh());
        }
        
        
    }

    private IEnumerator DissolveSkinnedMesh()
    {   
        if(skinnedMaterials.Length > 0)
        {
            float currentDissolve = 0;
            
            while (currentDissolve < 1f)
            {
                currentDissolve += dissolveRate;
                currentDissolve = Mathf.Clamp01(currentDissolve);

                for (int i = 0 ; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat(DissolveAmountID, currentDissolve);
                }

                yield return new WaitForSeconds(RefreshRate);
            }

        }

        
    }

    private IEnumerator RedissolveSkinnedMesh()
    {
        if (skinnedMaterials.Length > 0)
        {
            float currentDissolve = 1;

            while (currentDissolve > 0f)
            {
                currentDissolve -= dissolveRate;
                currentDissolve = Mathf.Clamp01(currentDissolve);

                for (int i = 0; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat(DissolveAmountID, currentDissolve);
                }

                yield return new WaitForSeconds(RefreshRate);
            }

        }
    }

    public void ResetDissolver()
    {
        StopAllCoroutines();
        isDissolving = false;

        // Reset dissolve amount on all instances
        foreach (var material in skinnedMaterials)
        {
            material.SetFloat(DissolveAmountID, 0f);
        }

        // Restore original materials
        skinnedMesh.materials = defaultMaterials;
    }

    private void OnDisable()
    {
        ResetDissolver();
    }

    private void OnDestroy()
    {
        
        if (skinnedMaterials != null)
        {
            foreach (var material in skinnedMaterials)
            {
                if (material != null)
                {
                    Destroy(material);
                }
            }
        }
    }
}

