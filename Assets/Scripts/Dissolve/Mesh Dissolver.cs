using System;
using System.Collections;
using UnityEngine;
public class MeshDissolver : MonoBehaviour
{
    [Header("References")]
    public MeshRenderer [] Meshes;
    public Material DissolveMaterial;
    public bool Dissolve;
    private bool isDissolving;

    [SerializeField] private Material[] defaultMaterials;
   
    [SerializeField] private float dissolveRate = 0.0125f;
    [SerializeField] private float RefreshRate = 0.025f;

    private static readonly int DissolveAmountID = Shader.PropertyToID("_Dissolve_Amount");

    private void Awake()
    {
        if (DissolveMaterial == null)
        {
            Debug.Log("Dissolve Material not Assigned");
        }

        Meshes = GetComponentsInChildren<MeshRenderer>();
        defaultMaterials = new Material[Meshes.Length];
        

        for (int i = 0; i < Meshes.Length; i++)
        {
            defaultMaterials[i] = Meshes[i].material;
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
    
    public void ResetDissolver()
    {
        StopAllCoroutines();
        isDissolving = false;

        // Restore original material to  Meshrenderers
        for (int i = 0; i < Meshes.Length; i++)
        {
            Meshes[i].material = defaultMaterials[i];
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
    private void StartDissolver()
    {
        if (!isDissolving)
        {   
            isDissolving = true;
            foreach (MeshRenderer renderer in Meshes)
            {
                renderer.material = DissolveMaterial;
            }
            StartCoroutine(DissolveMesh());
        }


    }

    private IEnumerator DissolveMesh()
    {
        if (Meshes.Length > 0)
        {
            float currentDissolve = 0;

            while (currentDissolve < 1f)
            {
                currentDissolve += dissolveRate;
                currentDissolve = Mathf.Clamp01(currentDissolve);

                for (int i = 0; i < Meshes.Length; i++)
                {
                    Meshes[i].material.SetFloat(DissolveAmountID, currentDissolve);
                }

                yield return new WaitForSeconds(RefreshRate);
            }

        }


    }
    private void OnDisable()
    {
        ResetDissolver();
    }
}
