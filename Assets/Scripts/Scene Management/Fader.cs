using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    private CanvasGroup group;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    public void FadeOutImmediately()
    {
        group.alpha = 1.0f;
    }

    public IEnumerator FadeOut(float time)
    {
        group.blocksRaycasts = true;
        yield return FadeRoutine(1, time);
    }

    public IEnumerator FadeIn(float time)
    {
        group.blocksRaycasts = false;
        yield return FadeRoutine(0, time);
    }

    private IEnumerator FadeRoutine(float target, float time)
    {
        if (Mathf.Approximately(group.alpha, target))
            yield break;

        while (!Mathf.Approximately(group.alpha, target))
        {
            group.alpha = Mathf.MoveTowards(group.alpha, target, Time.unscaledDeltaTime / time);
            yield return null;
        }
    }
}
