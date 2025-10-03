using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FullscreenEffectController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fullScreenDisplayTime = 1.0f;
    [SerializeField] private float effectFadeOutTime = 0.5f;
    [SerializeField] private float hitEffectDuration = 0.3f;

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

    // Default values
    private float defaulteffectPower;

    // Coroutines
    private Coroutine ailmentCoroutine;
    private Coroutine hitCoroutine;

    // Current effect
    private Material effectMaterial;

    [SerializeField] private float transitionEffectPower = 0f; // visible = low
    [SerializeField] private float effectFaderRate = 0.5f;
    [SerializeField] private float refreshRate = 0.02f;
    private float currenteffectPower;

    private void Start()
    {
        fullScreenEffect.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayHitEffect();
        }
    }

    public void ActivateFullscreenEffect(AilmentType ailmentType)
    {
        switch (ailmentType)
        {
            case AilmentType.Ignis: effectMaterial = FireEffectMaterial; break;
            case AilmentType.Gaia: effectMaterial = DendroEffectMaterial; break;
            case AilmentType.Blitz: effectMaterial = ElectricEffectMaterial; break;
            case AilmentType.Frost: effectMaterial = IceEffectMaterial; break;
            case AilmentType.Hex: effectMaterial = DarkEffectMaterial; break;
            case AilmentType.Radiance: effectMaterial = HolyEffectMaterial; break;
        }

        defaulteffectPower = effectMaterial.GetFloat(effectPower);

        if (ailmentCoroutine != null) StopCoroutine(ailmentCoroutine);
        ailmentCoroutine = StartCoroutine(StartFullscreenEffect(effectMaterial));
    }

    public void DeactivateFullscreenEffect()
    {
        if (ailmentCoroutine != null) StopCoroutine(ailmentCoroutine);
        ailmentCoroutine = StartCoroutine(EndFullscreenEffect(effectMaterial));
    }

    private IEnumerator StartFullscreenEffect(Material material)
    {
        fullScreenEffect.SetActive(true);

        currenteffectPower = transitionEffectPower;
        while (currenteffectPower < defaulteffectPower)
        {
            currenteffectPower = Mathf.Clamp(currenteffectPower + effectFaderRate, transitionEffectPower, defaulteffectPower);
            material.SetFloat(effectPower, currenteffectPower);
            fullScreenEffect.passMaterial = material; // refresh assignment
            yield return new WaitForSeconds(refreshRate);
        }

        yield return new WaitForSeconds(fullScreenDisplayTime);
        DeactivateFullscreenEffect();
        ailmentCoroutine = null;
    }

    private IEnumerator EndFullscreenEffect(Material material)
    {
        while (currenteffectPower > transitionEffectPower)
        {
            currenteffectPower = Mathf.Clamp(currenteffectPower - effectFaderRate, transitionEffectPower, defaulteffectPower);
            material.SetFloat(effectPower, currenteffectPower);
            fullScreenEffect.passMaterial = material; // refresh assignment
            yield return new WaitForSeconds(refreshRate);
        }

        material.SetFloat(effectPower, defaulteffectPower);
        fullScreenEffect.passMaterial = null;
        fullScreenEffect.SetActive(false);
        ailmentCoroutine = null;
    }

    public void PlayHitEffect()
    {
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);
        hitCoroutine = StartCoroutine(HitEffectRoutine());
    }

    private IEnumerator HitEffectRoutine()
    {
        fullScreenEffect.SetActive(true);

        float defaultPower = HitEffectMaterial.GetFloat(effectPower);
        float power = defaultPower;

        // Flash = decrease power
        while (power > transitionEffectPower)
        {
            power = Mathf.Max(transitionEffectPower, power - effectFaderRate * 5f);
            HitEffectMaterial.SetFloat(effectPower, power);
            fullScreenEffect.passMaterial = HitEffectMaterial; // refresh assignment
            yield return new WaitForSeconds(refreshRate);
        }

        yield return new WaitForSeconds(hitEffectDuration);

        // Fade back = increase to default
        while (power < defaultPower)
        {
            power = Mathf.Min(defaultPower, power + effectFaderRate * 5f);
            HitEffectMaterial.SetFloat(effectPower, power);
            fullScreenEffect.passMaterial = HitEffectMaterial; // refresh assignment
            yield return new WaitForSeconds(refreshRate);
        }

        HitEffectMaterial.SetFloat(effectPower, defaultPower);

        if (ailmentCoroutine == null)
        {
            fullScreenEffect.passMaterial = null;
            fullScreenEffect.SetActive(false);
        }

        hitCoroutine = null;
    }

    private void OnDisable()
    {
        if (effectMaterial != null)
            effectMaterial.SetFloat(effectPower, defaulteffectPower);

        HitEffectMaterial.SetFloat(effectPower, HitEffectMaterial.GetFloat(effectPower));
        fullScreenEffect.passMaterial = null;
        fullScreenEffect.SetActive(false);
    }
}
