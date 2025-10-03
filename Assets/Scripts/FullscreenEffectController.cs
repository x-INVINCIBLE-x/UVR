using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;
using System.Collections;
using System.Collections.Generic;

public class FullscreenEffectController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fullScreenDisplayTime = 1.0f;
    [SerializeField] private float effectFadeOutTime = 0.5f;
    [SerializeField] private float hitEffectDuration = 0.3f; // how long hit flash stays

    [Header("References")]
    [SerializeField] private FullScreenPassRendererFeature fullScreenEffect;
    [SerializeField] private Material FireEffectMaterial;
    [SerializeField] private Material IceEffectMaterial;
    [SerializeField] private Material ElectricEffectMaterial;
    [SerializeField] private Material DendroEffectMaterial;
    [SerializeField] private Material HolyEffectMaterial;
    [SerializeField] private Material DarkEffectMaterial;
    [SerializeField] private Material HitEffectMaterial;

    // Shader property IDs
    private static readonly int effectIntensity = Shader.PropertyToID("_Vignette_Intensity");
    private static readonly int effectPower = Shader.PropertyToID("_Vignette_Power");

    // Default values of effect material
    private float defaulteffectIntensity;
    private float defaulteffectPower;

    // Coroutine reference
    private Coroutine fullScreenCorountine;

    // Current material reference
    private Material effectMaterial;

    // Values to set for shader
    [SerializeField] private float transitionEffectPower = 25f;
    [SerializeField] private float effectFaderRate = 0.0125f;
    [SerializeField] private float refreshRate = 0.025f;
    private float currenteffectPower;

    private void Start()
    {
        fullScreenEffect.SetActive(false);
    }

    public void ActivateFullscreenEffect(AilmentType ailmentType)
    {
        switch (ailmentType)
        {
            case AilmentType.Ignis:
                effectMaterial = FireEffectMaterial;
                break;
            case AilmentType.Gaia:
                effectMaterial = DendroEffectMaterial;
                break;
            case AilmentType.Blitz:
                effectMaterial = ElectricEffectMaterial;
                break;
            case AilmentType.Frost:
                effectMaterial = IceEffectMaterial;
                break;
            case AilmentType.Hex:
                effectMaterial = DarkEffectMaterial;
                break;
            case AilmentType.Radiance:
                effectMaterial = HolyEffectMaterial;
                break;
        }

        defaulteffectIntensity = effectMaterial.GetFloat("_Vignette_Intensity");
        defaulteffectPower = effectMaterial.GetFloat("_Vignette_Power");

        if (fullScreenCorountine != null)
        {
            StopCoroutine(fullScreenCorountine);
        }
        fullScreenCorountine = StartCoroutine(StartFullscreenEffect(effectMaterial));
    }

    public void DeactivateFullscreenEffect()
    {
        if (fullScreenCorountine != null)
        {
            StopCoroutine(fullScreenCorountine);
        }
        fullScreenCorountine = StartCoroutine(EndFullscreenEffect(effectMaterial));
    }

    private IEnumerator StartFullscreenEffect(Material material)
    {
        fullScreenEffect.SetActive(true);
        fullScreenEffect.passMaterial = material;

        material.SetFloat(effectPower, transitionEffectPower);
        currenteffectPower = transitionEffectPower;

        while (currenteffectPower > defaulteffectPower)
        {
            currenteffectPower -= effectFaderRate;
            material.SetFloat(effectPower, currenteffectPower);
            yield return new WaitForSeconds(refreshRate);
        }

        // Wait for configured display time, then fade out automatically
        yield return new WaitForSeconds(fullScreenDisplayTime);
        DeactivateFullscreenEffect();

        fullScreenCorountine = null;
    }

    private IEnumerator EndFullscreenEffect(Material material)
    {
        while (currenteffectPower < transitionEffectPower)
        {
            currenteffectPower += effectFaderRate;
            material.SetFloat(effectPower, currenteffectPower);
            yield return new WaitForSeconds(refreshRate);
        }

        fullScreenCorountine = null;

        // Restore defaults and disable effect
        material.SetFloat(effectPower, defaulteffectPower);
        fullScreenEffect.passMaterial = null; // clear reference
        fullScreenEffect.SetActive(false);
    }

    /// <summary>
    /// Play a quick flash effect using HitEffectMaterial (for damage feedback).
    /// </summary>
    public void PlayHitEffect()
    {
        if (fullScreenCorountine != null)
        {
            StopCoroutine(fullScreenCorountine);
        }
        fullScreenCorountine = StartCoroutine(HitEffectRoutine());
    }

    private IEnumerator HitEffectRoutine()
    {
        fullScreenEffect.SetActive(true);
        fullScreenEffect.passMaterial = HitEffectMaterial;

        // Cache defaults
        float defaultPower = HitEffectMaterial.GetFloat(effectPower);

        // Fade in
        float power = defaultPower;
        while (power < transitionEffectPower)
        {
            power += effectFaderRate * 2f; // faster flash
            HitEffectMaterial.SetFloat(effectPower, power);
            yield return new WaitForSeconds(refreshRate);
        }

        // Hold at peak
        yield return new WaitForSeconds(hitEffectDuration);

        // Fade out
        while (power > defaultPower)
        {
            power -= effectFaderRate * 2f;
            HitEffectMaterial.SetFloat(effectPower, power);
            yield return new WaitForSeconds(refreshRate);
        }

        // Restore
        HitEffectMaterial.SetFloat(effectPower, defaultPower);
        fullScreenEffect.passMaterial = null;
        fullScreenEffect.SetActive(false);
        fullScreenCorountine = null;
    }

    private void OnDisable()
    {
        if (effectMaterial != null)
        {
            effectMaterial.SetFloat(effectPower, defaulteffectPower);
        }

        if (HitEffectMaterial != null)
        {
            HitEffectMaterial.SetFloat(effectPower, 0f);
        }

        fullScreenEffect.passMaterial = null;
        fullScreenEffect.SetActive(false);
    }
}
