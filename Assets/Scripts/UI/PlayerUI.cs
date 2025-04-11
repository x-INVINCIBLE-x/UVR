using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Ability Duration UI")]
    [SerializeField] private Image durationFill;
    [SerializeField] private GameObject durationSegmentParent;

    private Transform[] durationSegments;
    private Coroutine cooldownCoroutine;

    private void Awake()
    {
        InitializeAbilityDurationUI();
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
}
