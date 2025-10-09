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
    [SerializeField] private float wipeSizeSpeed = 2f; // Adjust for desired animation speed

    private RecallSettings recallEffect;
    private Coroutine wipeCoroutine;

    private void Start()
    {
        if (recallVolume != null)
            recallVolume.profile.TryGet(out recallEffect);

        if (recallEffect == null)
        {
            Debug.LogError("RecallSettings not found in Volume Profile!");
            enabled = false;
            return;
        }

        // Initialize values
        recallEffect.active = false;
        recallEffect.wipeSize.overrideState = true;
        recallEffect.wipeOriginPoint.overrideState = true;
        recallEffect.wipeSize.value = 0f;
        recallEffect.noiseScale.value = 100f;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Z) && wipeCoroutine == null)
    //    {
    //        wipeCoroutine = StartCoroutine(TimeStopEffect(true));
    //    }

    //    if (Input.GetKeyDown(KeyCode.X) && wipeCoroutine == null)
    //    {
    //        wipeCoroutine = StartCoroutine(TimeStopEffect(false));
    //    }
    //}

    public void StartTimeEffect() => StartCoroutine(TimeStopEffect(true));
    public void StopTimeEffect() => StartCoroutine(TimeStopEffect(false));

    private IEnumerator TimeStopEffect(bool status)
    {
        float startSize = recallEffect.wipeSize.value;
        float targetSize = status ? 2.5f : 0f;

        yield return null;

        // If activating, force start from 0 before enabling the effect
        if (status)
        {
            recallEffect.wipeSize.value = 0f;
            recallEffect.noiseScale.value = 100f;
            recallEffect.active = true;
        }

        AudioClip clipToPlay = status ? timeStopStart : timeStopEnd;
        AudioManager.Instance.PlaySFX2d(timeAudioSource, clipToPlay, 1f);

        // Animate wipeSize smoothly
        while (Mathf.Abs(recallEffect.wipeSize.value - targetSize) > 0.01f)
        {
            yield return null; // smoother than WaitForEndOfFrame
            recallEffect.wipeSize.value = Mathf.MoveTowards(recallEffect.wipeSize.value, targetSize, wipeSizeSpeed * Time.unscaledDeltaTime);

            // Animate noise proportionally to wipeSize
            recallEffect.noiseScale.value = Mathf.Lerp(100f, 199f, Mathf.InverseLerp(0f, 2.5f, recallEffect.wipeSize.value));
        }

        // If deactivating, smoothly hide effect
        if (!status)
            recallEffect.active = false;

        wipeCoroutine = null;
    }
}
