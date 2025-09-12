using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MeshDissolver : Dissolver
{
    [Header("References")]
    public MeshRenderer[] Meshes;
    [SerializeField] private float dissolveRate = 0.0125f;
    [SerializeField] private float refreshRate = 0.025f;

    [SerializeField] private float impactDissolveRate = 0.0125f;
    [SerializeField] private float impactDissolveThreshold = 0.5f;
    [SerializeField] private float currentDissolve;


    private bool isDissolving;
    private Coroutine dissolveCoroutine;

    private MaterialPropertyBlock mpb;
    private static readonly int DissolveAmountID = Shader.PropertyToID("_Dissolve_Amount");


    private void Awake()
    {   
        if (Meshes == null || Meshes.Length == 0)
        {
            Meshes = GetComponentsInChildren<MeshRenderer>();
        }

        if (Meshes.Length == 0)
        {
            Debug.LogWarning("No MeshRenderers found", this);
            return;
        }

        mpb = new MaterialPropertyBlock();
        currentDissolve = 0f;// Intialize to 0 when starting
        SetDissolveAmount(currentDissolve);
    }

    private List<Material> dissolvingMaterials = new List<Material>();

   
    private void SetDissolveAmount(float amount)
    {
        currentDissolve = Mathf.Clamp01(amount);

        // Use the cached list
        foreach(MeshRenderer renderer in Meshes)
        {
            if (renderer == null) continue;

            renderer.GetPropertyBlock(mpb);
            mpb.SetFloat(DissolveAmountID, currentDissolve);
            renderer.SetPropertyBlock(mpb);
        }
    }


    public override void StartDissolve()
    {
        if (dissolveCoroutine != null)
        {
            StopCoroutine(dissolveCoroutine);
        }

        isDissolving = true;
        dissolveCoroutine = StartCoroutine(DissolveMesh());
    }

    // This method does both impact dissolve and impact redissolve in the given time
    // Used where we want to dissolve and redissolve
    public override void StartImpactDissolve(float duration)
    {
        StartCoroutine(ImpactDissolveRoutine(duration));
    }

    public override void ImpactPartialDissolve()
    {
        if (dissolveCoroutine != null)
        {
            StopCoroutine(dissolveCoroutine);
        }
           
        isDissolving = true;
        dissolveCoroutine = StartCoroutine(DissolveMesh(impactDissolveThreshold, false));

    }

    public override void ImpactPartialRedissolve()
    {
        if (dissolveCoroutine != null)
        {
            StopCoroutine(dissolveCoroutine);
        }

        isDissolving = true;
        dissolveCoroutine = StartCoroutine(RedissolveMesh());
    }
    public void ResetDissolver()
    {
        isDissolving = false;
        currentDissolve = 0f;
        SetDissolveAmount(currentDissolve);
        StopAllCoroutines();
    }

    private IEnumerator ImpactDissolveRoutine(float dissolveDuration)
    {
        ImpactPartialDissolve();
        yield return dissolveCoroutine;
        yield return new WaitForSeconds(dissolveDuration);
        if (dissolveCoroutine == null)
            ImpactPartialRedissolve();
    }


    // Important disclaimer if the parameter dissolve is true ,then normal disolve
    // if dissolve is false then we do a specified threshold dissolve
    private IEnumerator DissolveMesh(float dissolveThreshold = 1f, bool dissolve = true)
    {
        if (Meshes.Length > 0)
        {
            float currentDissolveRate = dissolve ? dissolveRate : impactDissolveRate;

            while (currentDissolve < dissolveThreshold)
            {
                currentDissolve += currentDissolveRate;
                currentDissolve = Mathf.Clamp01(currentDissolve);
                SetDissolveAmount(currentDissolve);
                yield return new WaitForSeconds(refreshRate);
            }
            SetDissolveAmount(dissolveThreshold);// Force to threhold
            // reset isDissolving for next calls
            isDissolving = false;
            dissolveCoroutine = null;
        }
    }

    // Redissolves the Mesh renderers to 0 dissolve rate
    private IEnumerator RedissolveMesh(float dissolveThreshold = 0.0f)
    {
        if (Meshes.Length > 0)
        {
            while (currentDissolve > dissolveThreshold)
            {
                currentDissolve -= impactDissolveRate;
                currentDissolve = Mathf.Clamp01(currentDissolve);
                SetDissolveAmount(currentDissolve);
                yield return new WaitForSeconds(refreshRate);
            }

            SetDissolveAmount(dissolveThreshold);// Force to threhold
            // reset isDissolving for next calls
            isDissolving = false;
            dissolveCoroutine = null;
        }
    }

    private void OnDisable()
    {
        ResetDissolver();
    }


}

