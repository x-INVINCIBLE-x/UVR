using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("Health UI")]
    [SerializeField] private SkinnedMeshRenderer[] healthRenderes;
    private static readonly int DissolveAmountID = Shader.PropertyToID("_Horizontal_Dissolve_Amount");
    private const float dissolveMaxValue = 0.725f;
    private const float dissolveMinValue = 0.990f;

    [Header("Ability Duration UI")]
    [SerializeField] private Image durationFill;
    [SerializeField] private GameObject durationSegmentParent;

    [Header("Full scrreen effect")]
    [SerializeField] private FullscreenEffectController fullScreenEffect;
    
    private Transform[] durationSegments;
    private Coroutine cooldownCoroutine;

    private void Awake()
    {
        InitializeAbilityDurationUI();

        foreach (SkinnedMeshRenderer renderer in healthRenderes)
        {
            if (renderer != null && renderer.material != null)
            {
                renderer.material.SetFloat(DissolveAmountID, dissolveMaxValue);
            }
        }
    }

    private void Start()
    {
        player = PlayerManager.instance.Player;
        player.Stats.OnHealthChanged += UpdateHealthUI;

        player.Stats.OnAilmentStatusChange += HandleStatusChange;
    }

    private void HandleStatusChange(AilmentType type, bool isActivated, float effectAmount)
    {
        if (isActivated == true)
        {
            fullScreenEffect.ActivateFullscreenEffect(type);
            player.Stats.GetAilmentStatus(type).AilmentEffectEnded += HandleEffectEnd;
        }
    }

    private void HandleEffectEnd(AilmentType type)
    {
        fullScreenEffect.DeactivateFullscreenEffect();
        player.Stats.GetAilmentStatus(type).AilmentEffectEnded -= HandleEffectEnd;
    }

    private void UpdateHealthUI(float normalizedValue)
    {
        float diff = dissolveMinValue - dissolveMaxValue;
        float dissolveValue = dissolveMinValue - (diff * normalizedValue);

        foreach (SkinnedMeshRenderer renderer in healthRenderes)
        {
            if (renderer != null && renderer.material != null)
            {
                renderer.material.SetFloat(DissolveAmountID, dissolveValue);
            }
        }
    }

    private void InitializeAbilityDurationUI()
    {
        int count = durationSegmentParent.transform.childCount;
        durationSegments = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            durationSegments[i] = durationSegmentParent.transform.GetChild(i);
            durationSegments[i].gameObject.SetActive(false);
        }

        durationSegmentParent.SetActive(false);
        durationFill.fillAmount = 0;
    }

    public void StartAbilityDurationCooldown(float duration)
    {
        if (cooldownCoroutine != null)
            StopCoroutine(cooldownCoroutine);

        cooldownCoroutine = StartCoroutine(DurationCooldownRoutine(duration));
    }

    public void StopAbilityDurationCooldown()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }

        UpdateDurationCooldownUI(1f);
    }

    private IEnumerator DurationCooldownRoutine(float duration)
    {
        float startTime = Time.unscaledTime;
        float endTime = startTime + duration;
        durationSegmentParent.SetActive(true);

        while (Time.unscaledTime < endTime)
        {
            float remainingTime = endTime - Time.unscaledTime;
            UpdateDurationCooldownUI(remainingTime / duration);
            yield return null;
        }

        UpdateDurationCooldownUI(0f);
        durationSegmentParent.SetActive(false);
        cooldownCoroutine = null;
    }

    private void UpdateDurationCooldownUI(float fillAmount)
    {
        durationFill.fillAmount = fillAmount;
        int activeSegments = Mathf.RoundToInt(fillAmount * durationSegments.Length);

        for (int i = 0; i < durationSegments.Length; i++)
            durationSegments[i].gameObject.SetActive(i < activeSegments);
    }

    private void OnDisable()
    {
        foreach (SkinnedMeshRenderer renderer in healthRenderes)
        {
            if (renderer != null && renderer.material != null)
            {
                renderer.material.SetFloat(DissolveAmountID, dissolveMaxValue);
            }
        }
    }

    private void OnDestroy()
    {
        if (player != null && player.Stats != null)
        {
            player.Stats.OnHealthChanged -= UpdateHealthUI;
        }
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }
    }
}
