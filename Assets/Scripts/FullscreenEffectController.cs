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
    private Coroutine fullScreenCorountine;
    
    // Material Reference
    private Material effectType;

    // Values to Set For Shader
    [SerializeField] private float transitioneffectPower = 25f;
    [SerializeField] private float effectFaderRate = 0.0125f; // Addtion value for increase per refresh rate
    [SerializeField] private float refreshRate = 0.025f; // Coroutine time value
    private float currenteffectPower;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ActivateFullscreenEffect(AilmentType.Ignis);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            DeactivateFullscreenEffect();
        }
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
        
        if(fullScreenCorountine!= null)
        {
            StopCoroutine(fullScreenCorountine);
        }
        fullScreenCorountine = StartCoroutine(StartFullscreenEffect(effectType));
    }

    public void DeactivateFullscreenEffect()
    {
        if (fullScreenCorountine != null)
        {
            StopCoroutine(fullScreenCorountine);
        }
        fullScreenCorountine = StartCoroutine(EndFullscreenEffect(effectType));
    }

    private IEnumerator StartFullscreenEffect(Material effectMaterial)
    {   
        fullScreenEffect.SetActive(true);
        fullScreenEffect.passMaterial = effectMaterial;

        effectMaterial.SetFloat(effectPower, transitioneffectPower);
        currenteffectPower = transitioneffectPower; // Set to transition effect power 

        while (currenteffectPower > effectPower) // From transtion effect power to effect power
        {
            currenteffectPower -= effectFaderRate;
            effectMaterial.SetFloat(effectPower, currenteffectPower);
            yield return new WaitForSeconds(refreshRate);
        }

        fullScreenCorountine = null;
    }

    private IEnumerator EndFullscreenEffect(Material effectMaterial)
    {         
        while (currenteffectPower < effectPower) // From current effect power to effect power (default)
        {
            currenteffectPower += effectFaderRate;
            effectMaterial.SetFloat(effectPower, currenteffectPower);
            yield return new WaitForSeconds(refreshRate);
        }

        fullScreenCorountine = null;
        fullScreenEffect.SetActive(false);
    }

    private void OnDisable()
    {
        effectType.SetFloat(effectIntensity, defaulteffectIntensity);
        effectType.SetFloat(effectPower, defaulteffectPower);
    }
}
