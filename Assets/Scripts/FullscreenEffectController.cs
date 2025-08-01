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

    [SerializeField] private ScriptableRendererFeature fullScreenEffect;
    [SerializeField] private List<Material> efffectMaterials;

    // Shader Variables (Property) 
    private static readonly int effectIntensity = Shader.PropertyToID("_Vignette_Intensity");
    private static readonly int effectPower = Shader.PropertyToID("_Vignette_Power");

    // Default value of the effect material
    private int defaulteffectIntensity; 
    private int defaulteffectPower;

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
    private IEnumerator StartFullscreenEffect()
    {
        yield return null;
    }
}
