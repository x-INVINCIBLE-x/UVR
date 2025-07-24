using System;
using System.Collections;
using UnityEngine;

public class SkinnedMeshDissolver : MonoBehaviour
{
    [Header("References")]
    public SkinnedMeshRenderer skinnedMesh;
    public Material DissolveMaterial;
    public bool Dissolve; // remove this only for testing
    private bool isDissolving;

    [SerializeField] private Material[] defaultMaterials;
    [SerializeField] private Material[] skinnedMaterials;

   
    [SerializeField] private float dissolveRate = 0.0125f;
    [SerializeField] private float RefreshRate = 0.025f;

    [SerializeField] private float impactDissolveRate = 0.0125f;
    [SerializeField] private float impactDissolveThreshold = 0.5f;
    [SerializeField] private float currentDissolve;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetDissolver();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            ImpactSkinnedPartialDissolve();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ImpactSkinnedPartialRedissolve();
        }




        // to remove this update statements(important)
        if (Dissolve)
        {
            //StartDissolver();
        }
        else
        {
            //ResetDissolver();
        }
    }

    // Public for external use
    public void ActivateSkinnedDissolver(bool dissolveStatus) // remove this only for testing
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

    public void ImpactSkinnedPartialDissolve()
    {
        if (!isDissolving)
        {
            isDissolving = true;
            skinnedMesh.materials = skinnedMaterials;
            StartCoroutine(DissolveSkinnedMesh(impactDissolveThreshold, false));
        }

    }

    public void ImpactSkinnedPartialRedissolve()
    {
        if (!isDissolving)
        {
            isDissolving = true;
            skinnedMesh.materials = skinnedMaterials;
            StartCoroutine(RedissolveSkinnedMesh());
        }

    }

    // Important disclaimer if the parameter dissolve is true ,then normal disolve
    // if dissolve is false then we do a specified threshold dissolve
    private IEnumerator DissolveSkinnedMesh(float dissolveThreshold = 1f, bool dissolve = true)
    {   
        if(skinnedMaterials.Length > 0)
        {
            currentDissolve = 0;

            float currentDissolveRate = dissolve ? dissolveRate : impactDissolveRate;

            while (currentDissolve < dissolveThreshold)
            {
                currentDissolve += currentDissolveRate;
                currentDissolve = Mathf.Clamp01(currentDissolve);

                for (int i = 0 ; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat(DissolveAmountID, currentDissolve);
                }

                yield return new WaitForSeconds(RefreshRate);
            }

            // Reset isDissolving for next calls
            isDissolving = false;
        }
    }

    // Redissolves the skinned mesh renderers to 0 dissolve rate
    private IEnumerator RedissolveSkinnedMesh(float dissolveThreshold = 0.0f)
    {
        if (skinnedMaterials.Length > 0)
        {
           
            while (currentDissolve > dissolveThreshold)
            {
                currentDissolve -= impactDissolveRate;
                currentDissolve = Mathf.Clamp01(currentDissolve);

                for (int i = 0; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat(DissolveAmountID, currentDissolve);
                }

                yield return new WaitForSeconds(RefreshRate);
            }

            // Reset isDissolving for next calls
            isDissolving = false;
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
        StopAllCoroutines();
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

