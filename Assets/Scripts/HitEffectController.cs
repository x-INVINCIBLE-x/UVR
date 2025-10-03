using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class HitEffectController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FullScreenPassRendererFeature fullScreenEffect;
    [SerializeField] private Material hitEffectMaterial;

    [Header("Hit Effect Settings")]
    [SerializeField] private float hitEffectDuration = 0.3f; // how long it holds at visible power
    [SerializeField] private float hitEffectFadeRate = 0.5f; // how fast fade happens
    [SerializeField] private float refreshRate = 0.025f;     // coroutine step time
    [SerializeField] private float invisiblePower = 25f;     // fully invisible
    [SerializeField] private float visiblePower = 5f;        // visible flash

    private static readonly int effectPower = Shader.PropertyToID("_Vignette_Power");
    private Coroutine hitCoroutine;

    /// <summary>
    /// Triggers the hit effect flash.
    /// </summary>
    public void PlayHitEffect()
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }
        hitCoroutine = StartCoroutine(HitEffectRoutine());
    }

    private IEnumerator HitEffectRoutine()
    {
        fullScreenEffect.SetActive(true);
        fullScreenEffect.passMaterial = hitEffectMaterial;

        float power = invisiblePower;
        hitEffectMaterial.SetFloat(effectPower, power);

        // Fade in (from invisible to visible)
        while (power > visiblePower)
        {
            power -= hitEffectFadeRate;
            hitEffectMaterial.SetFloat(effectPower, power);
            yield return new WaitForSeconds(refreshRate);
        }

        // Hold at visible level
        yield return new WaitForSeconds(hitEffectDuration);

        // Fade back to invisible
        while (power < invisiblePower)
        {
            power += hitEffectFadeRate;
            hitEffectMaterial.SetFloat(effectPower, power);
            yield return new WaitForSeconds(refreshRate);
        }

        // Reset and disable
        hitEffectMaterial.SetFloat(effectPower, invisiblePower);
        fullScreenEffect.passMaterial = null;
        fullScreenEffect.SetActive(false);
        hitCoroutine = null;
    }

    private void OnDisable()
    {
        if (hitEffectMaterial != null)
        {
            hitEffectMaterial.SetFloat(effectPower, invisiblePower);
        }
        fullScreenEffect.passMaterial = null;
        fullScreenEffect.SetActive(false);
    }
}
