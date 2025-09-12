using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshDissolver : Dissolver
{
    [Header("References")]
    public SkinnedMeshRenderer[] skinnedMeshes;

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
        if (skinnedMeshes == null || skinnedMeshes.Length == 0)
        {
            skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        }

        mpb = new MaterialPropertyBlock();
        currentDissolve = 0f;
        SetDissolveAmount(0f); // Initialize dissolve to zero
    }

    private void SetDissolveAmount(float amount)
    {
        currentDissolve = Mathf.Clamp01(amount);

        foreach (SkinnedMeshRenderer renderer in skinnedMeshes)
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
            StopCoroutine(dissolveCoroutine);

        isDissolving = true;
        dissolveCoroutine = StartCoroutine(DissolveSkinnedMesh());
    }

    public override void StartImpactDissolve(float duration)
    {
        StartCoroutine(ImpactDissolveRoutine(duration));
    }

    public override void ImpactPartialDissolve()
    {
        if (dissolveCoroutine != null)
            StopCoroutine(dissolveCoroutine);

        isDissolving = true;
        dissolveCoroutine = StartCoroutine(DissolveSkinnedMesh(impactDissolveThreshold, false));
    }

    public override void ImpactPartialRedissolve()
    {
        if (dissolveCoroutine != null)
            StopCoroutine(dissolveCoroutine);

        isDissolving = true;
        dissolveCoroutine = StartCoroutine(RedissolveSkinnedMesh());
    }

    private IEnumerator ImpactDissolveRoutine(float dissolveDuration)
    {
        ImpactPartialDissolve();
        yield return dissolveCoroutine;
        yield return new WaitForSeconds(dissolveDuration);

        if (dissolveCoroutine == null)
            ImpactPartialRedissolve();
    }

    private IEnumerator DissolveSkinnedMesh(float dissolveThreshold = 1f, bool dissolve = true)
    {
        if (skinnedMeshes.Length > 0)
        {
            currentDissolve = 0f;
            float currentDissolveRate = dissolve ? dissolveRate : impactDissolveRate;

            while (currentDissolve < dissolveThreshold)
            {
                currentDissolve += currentDissolveRate;
                currentDissolve = Mathf.Clamp01(currentDissolve);
                SetDissolveAmount(currentDissolve);
                yield return new WaitForSeconds(refreshRate);
            }

            SetDissolveAmount(dissolveThreshold);
            isDissolving = false;
            dissolveCoroutine = null;
        }
    }

    private IEnumerator RedissolveSkinnedMesh(float dissolveThreshold = 0.0f)
    {
        if (skinnedMeshes.Length > 0)
        {
            while (currentDissolve > dissolveThreshold)
            {
                currentDissolve -= impactDissolveRate;
                currentDissolve = Mathf.Clamp01(currentDissolve);
                SetDissolveAmount(currentDissolve);
                yield return new WaitForSeconds(refreshRate);
            }

            SetDissolveAmount(dissolveThreshold);
            isDissolving = false;
            dissolveCoroutine = null;
        }
    }

    public void ResetDissolver()
    {
        isDissolving = false;
        currentDissolve = 0f;
        SetDissolveAmount(currentDissolve);
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        ResetDissolver();
    }
}
