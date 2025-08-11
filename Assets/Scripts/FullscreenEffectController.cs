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
    [SerializeField] private Material FireEffectMaterial;
    [SerializeField] private Material IceEffectMaterial;
    [SerializeField] private Material ElectricEffectMaterial;
    [SerializeField] private Material DendroEffectMaterial;
    [SerializeField] private Material HolyEffectMaterial;
    [SerializeField] private Material DarkEffectMaterial;
    
    // Shader Variables (Property) 
    private static readonly int effectIntensity = Shader.PropertyToID("_Vignette_Intensity");
    private static readonly int effectPower = Shader.PropertyToID("_Vignette_Power");

    // Default value of the effect material
    private float defaulteffectIntensity; 
    private float defaulteffectPower;

    // Coroutines 
    private Coroutine fullScreenCorountine;
    
    // Material Reference
    private Material effectMaterial;

    // Values to Set For Shader
    [SerializeField] private float transitionEffectPower = 25f;
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
        
    }

    private void Start()
    {
        fullScreenEffect.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ActivateFullscreenEffect(AilmentType.Frost);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            ActivateFullscreenEffect(AilmentType.Gaia);
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
            effectMaterial = FireEffectMaterial;
        }
        else if(ailmentType == AilmentType.Gaia)
        {
            effectMaterial = DendroEffectMaterial;
        }
        else if(ailmentType == AilmentType.Blitz)
        {
            effectMaterial = ElectricEffectMaterial;
        }
        else if(ailmentType == AilmentType.Frost)
        {
            effectMaterial = IceEffectMaterial;
        }
        else if(ailmentType == AilmentType.Hex)
        {
            effectMaterial = DarkEffectMaterial;
        }
        else if(ailmentType == AilmentType.Radiance)
        {
            effectMaterial = HolyEffectMaterial;
        }

        defaulteffectIntensity = effectMaterial.GetFloat("_Vignette_Intensity");
        defaulteffectPower = effectMaterial.GetFloat("_Vignette_Power");

        if (fullScreenCorountine!= null)
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
        currenteffectPower = transitionEffectPower; // Set to transition effect power 

        while (currenteffectPower > defaulteffectPower) // From transtion effect power to effect power
        {
            currenteffectPower -= effectFaderRate;
            material.SetFloat(effectPower, currenteffectPower);
            yield return new WaitForSeconds(refreshRate);
        }

        fullScreenCorountine = null;
    }

    private IEnumerator EndFullscreenEffect(Material material)
    {
        // Assuming we call the StartFullscreenEffect before  EndFullscreenEffect 
        // Or else we have to set the currenteffectPower to defaulteffectpower

        while (currenteffectPower < transitionEffectPower) // From current effect power to effect power (default)
        {
            currenteffectPower += effectFaderRate;
            material.SetFloat(effectPower, currenteffectPower);
            yield return new WaitForSeconds(refreshRate);
        }

        material.SetFloat(effectPower, defaulteffectPower); // Force set to original effect power value
        fullScreenCorountine = null;
        fullScreenEffect.SetActive(false);
    }

    private void OnDisable()
    {
        effectMaterial.SetFloat(effectPower, defaulteffectPower);       
    }
}
