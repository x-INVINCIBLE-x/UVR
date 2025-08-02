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

    [Header("References")]

    [SerializeField] private FullScreenPassRendererFeature fullScreenEffect;
    [SerializeField] private Material FireEffect;
    [SerializeField] private Material IceEffect;
    [SerializeField] private Material ElectricEffect;
    [SerializeField] private Material DendroEffect;
    [SerializeField] private Material HolyEffect;
    [SerializeField] private Material DarkEffect;
    
    // Shader Variables (Property) 
    private static readonly int effectIntensity = Shader.PropertyToID("_Vignette_Intensity");
    private static readonly int effectPower = Shader.PropertyToID("_Vignette_Power");

    // Default value of the effect material
    private int defaulteffectIntensity; 
    private int defaulteffectPower;

    // Coroutines 
    private Coroutine startCoroutine;
    private Coroutine endCoroutine;

    // Material Reference
    private Material effectType;

    //[SerializeField] private Color color;
    //[SerializeField] private float intensity = 8f;
    //[SerializeField] private float power = 4f;
    //[SerializeField] private float breathFrequency = 2f;
    //[SerializeField] private float breathIntensity = 0.6f;

    private void Awake()
    {
        defaulteffectIntensity = effectIntensity;
        defaulteffectPower = effectPower;
    }

    private void Start()
    {
        fullScreenEffect.SetActive(false);
    }

    public void ActivateFullscreenEffect(AilmentType ailmentType)
    {

        if(ailmentType == AilmentType.Ignis)
        {
            effectType = FireEffect;
        }
        else if(ailmentType == AilmentType.Gaia)
        {
            effectType = DendroEffect;
        }
        else if(ailmentType == AilmentType.Blitz)
        {
            effectType = ElectricEffect;
        }
        else if(ailmentType == AilmentType.Frost)
        {
            effectType = IceEffect;
        }
        else if(ailmentType == AilmentType.Hex)
        {
            effectType = DarkEffect;
        }
        else if(ailmentType == AilmentType.Radiance)
        {
            effectType = HolyEffect;
        }
        
        if(startCoroutine != null)
        {
            StopCoroutine(startCoroutine);
        }
        startCoroutine = StartCoroutine(StartFullscreenEffect(effectType));
    }

    public void DeactivateFullscreenEffect()
    {
        endCoroutine = StartCoroutine(EndFullscreenEffect(effectType));
    }

    private IEnumerator StartFullscreenEffect(Material effectMaterial)
    {   

        fullScreenEffect.SetActive(true);
        fullScreenEffect.passMaterial = effectMaterial;
        startCoroutine = null;
        yield return null;
    }

    private IEnumerator EndFullscreenEffect(Material effectMaterial)
    {
        fullScreenEffect.SetActive(false);
        fullScreenEffect.passMaterial = effectMaterial;
        endCoroutine = null;
        yield return null;
    }

    private void OnDisable()
    {
        effectType.SetFloat(effectIntensity, defaulteffectIntensity);
        effectType.SetFloat(effectPower, defaulteffectPower);
    }
}
