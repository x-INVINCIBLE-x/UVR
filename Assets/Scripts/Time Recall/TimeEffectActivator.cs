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

        recallEffect.active = false;
        recallEffect.wipeSize.overrideState = true;
        recallEffect.wipeOriginPoint.overrideState = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && wipeCoroutine == null)
        {
            wipeCoroutine = StartCoroutine(TimeStopEffect(true));
        }

        if (Input.GetKeyDown(KeyCode.X) && wipeCoroutine == null)
        {
            wipeCoroutine = StartCoroutine(TimeStopEffect(false));
        }
    }

    private IEnumerator TimeStopEffect(bool status)
    {
        recallEffect.active = true;
        AudioClip clipToPlay = status ? timeStopStart : timeStopEnd;
        AudioManager.Instance.PlaySFX2d(timeAudioSource, clipToPlay, 1f);

        float targetSize = status ? 2.5f : 0f;
        float startSize = recallEffect.wipeSize.value;

        while (Mathf.Abs(recallEffect.wipeSize.value - targetSize) > 0.01f)
        {
            yield return new WaitForEndOfFrame();
            recallEffect.wipeSize.value = Mathf.MoveTowards(recallEffect.wipeSize.value, targetSize, wipeSizeSpeed * Time.unscaledDeltaTime);
            recallEffect.noiseScale.value = Mathf.Lerp(100f, 199f, status ? 0.2f : 0.8f);
        }

        if (!status)
            recallEffect.active = false;

        wipeCoroutine = null;
    }
}
