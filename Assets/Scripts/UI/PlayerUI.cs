using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public class StatusColorCode
    {
        public AilmentType type;
        public Color color;
    }

    [SerializeField] private Player player;

    [Header("Health UI")]
    [SerializeField] private SkinnedMeshRenderer[] healthRenderes;
    [SerializeField] private SkinnedMeshRenderer[] statusRenderers;

    private static readonly int DissolveAmountID = Shader.PropertyToID("_Horizontal_Dissolve_Amount");
    private const float dissolveMaxValue = 0.725f;
    private const float dissolveMinValue = 0.990f;

    [Header("Ailment Display")]
    [SerializeField] private CharacterStatusVfx statusVfx;
    [SerializeField] private StatusColorCode[] statusColors;

    [Header("Ability Duration UI")]
    [SerializeField] private Image durationFill;
    [SerializeField] private GameObject durationSegmentParent;

    [Header("Hp Update Text")]
    [SerializeField] private CanvasGroup healthCanvas;
    [SerializeField] private TextMeshProUGUI healthText;
    private Coroutine healthRoutine;
    private float displayDuration = 1.5f;
    private float lastDamageValue = 0f;

    [Header("Level Up UI")]
    [SerializeField] private Transform levelupUIContainer;
    [SerializeField] private LevelupUIView levelupUI;
    [SerializeField] private AudioClip levelupSFX;

    [Header("Full scrreen effect")]
    [SerializeField] private HitEffectController hitEffectController;
    
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
        player.Stats.OnDamageTaken += HandleDamageTaken;
        player.Stats.OnDeath += HandleDeath;

        player.XP.OnLevelUp += HandleLevelup;
    }

    private void HandleDamageTaken(float damageAmount)
    {
        if (healthCanvas.alpha == 0)
        {
            lastDamageValue = 0;
        }

        lastDamageValue += damageAmount;
        healthText.text = "-" + lastDamageValue.ToString();
   
        healthCanvas.gameObject.SetActive(true);
        healthCanvas.alpha = 1;

        hitEffectController.PlayHitEffect();

        if (healthRoutine != null)
            StopCoroutine(healthRoutine);

        healthRoutine = StartCoroutine(FadeOutGroup(healthCanvas));
    }

    private IEnumerator FadeOutGroup(CanvasGroup group)
    {
        float timer = 0;
        while (timer < displayDuration)
        {
            timer += Time.deltaTime;
            float t = timer / displayDuration;
            float targetAlpha = Mathf.Lerp(0, 1, t);

            group.alpha = targetAlpha;
            yield return null;
        }

        group.alpha = 0;
        group.gameObject.SetActive(false);
        healthRoutine = null;
    }

    private void HandleLevelup(int newLevel)
    {
        LevelupUIView newView = Instantiate(levelupUI, levelupUIContainer);
        newView.SetLevelText((newLevel-1).ToString(), newLevel.ToString());
        AudioManager.Instance.PlaySystemSFX(levelupSFX);
        Destroy(newView.gameObject, 3f);
    }

    private void HandleStatusChange(AilmentType type, bool isActivated, float effectAmount)
    {   
        if (isActivated == true && statusVfx != null)
        {   
            statusVfx.SpawnStatusVFX(type, true);
            player.Stats.GetAilmentStatus(type).AilmentEffectEnded += HandleEffectEnd;
        }
    }

    private void HandleEffectEnd(AilmentType type)
    {   
        if (statusVfx != null)
        {
            statusVfx.SpawnStatusVFX(type, false);
        }
        //if (fullScreenEffect != null)
        //{
        //    fullScreenEffect.DeactivateFullscreenEffect();
        //    Debug.Log("Deactivated Fullscreen Effect for " + type);
        //}
        player.Stats.GetAilmentStatus(type).AilmentEffectEnded -= HandleEffectEnd;
    }

    private void HandleDeath()
    {
        //if (fullScreenEffect != null)
        //    fullScreenEffect.DeactivateFullscreenEffect();
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
