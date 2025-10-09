using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class TimeEffectActivator : MonoBehaviour
{
    [Header("References and Settings")]
    [SerializeField] private Volume recallVolume;
    [SerializeField] private AudioSource timeAudioSource;
    [SerializeField] private AudioClip timeStopStart;
    [SerializeField] private AudioClip timeStopEnd;
    [SerializeField] private float wipeSizeSpeed = 2f; // Animation speed

    private RecallSettings recallEffect;
    private Coroutine wipeCoroutine;

    private void Start()
    {
        if (recallVolume == null || recallVolume.profile == null)
        {
            Debug.LogError("Recall Volume or Profile is missing!");
            enabled = false;
            return;
        }

        if (!recallVolume.profile.TryGet(out recallEffect) || recallEffect == null)
        {
            Debug.LogError(" RecallSettings not found in Volume Profile!");
            enabled = false;
            return;
        }

        InitializeEffect();
    }

    private void InitializeEffect()
    {
        recallEffect.active = false;
        recallEffect.wipeSize.overrideState = true;
        recallEffect.wipeOriginPoint.overrideState = true;
        recallEffect.wipeSize.value = 0f;
        recallEffect.noiseScale.value = 100f;
    }

    public void StartTimeEffect()
    {
        if (wipeCoroutine != null) StopCoroutine(wipeCoroutine);
        wipeCoroutine = StartCoroutine(TimeStopEffect(true));
    }

    public void StopTimeEffect()
    {
        if (wipeCoroutine != null) StopCoroutine(wipeCoroutine);
        wipeCoroutine = StartCoroutine(TimeStopEffect(false));
    }

    private IEnumerator TimeStopEffect(bool status)
    {
        float targetSize = status ? 2.5f : 0f;

        if (status)
        {
            recallEffect.active = true;
            recallEffect.wipeSize.value = 0f;
            recallEffect.noiseScale.value = 100f;
        }

        // Play Audio
        AudioClip clipToPlay = status ? timeStopStart : timeStopEnd;
        if (clipToPlay != null && timeAudioSource != null)
            AudioManager.Instance.PlaySFX2d(timeAudioSource, clipToPlay, 1f);

        // Animate Transition
        while (Mathf.Abs(recallEffect.wipeSize.value - targetSize) > 0.01f)
        {
            recallEffect.wipeSize.value = Mathf.MoveTowards(
                recallEffect.wipeSize.value,
                targetSize,
                wipeSizeSpeed * Time.unscaledDeltaTime
            );

            yield return null;
        }

        // End Effect
        if (!status)
            recallEffect.active = false;

        wipeCoroutine = null;
    }

    private void OnDisable()
    {
        if (wipeCoroutine != null)
        {
            StopCoroutine(wipeCoroutine);
            wipeCoroutine = null;
        }

        if (recallEffect != null)
        {
            recallEffect.wipeSize.value = 0f;
            recallEffect.noiseScale.value = 100f;
            recallEffect.active = false;
        }
    }
}
