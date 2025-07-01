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
        return Fade(1, time);
    }

    public IEnumerator FadeIn(float time)
    {
        group.blocksRaycasts = false;   
        return Fade(0, time);
    }

    private IEnumerator Fade(float target, float time)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(FadeRoutine(target, time));
        yield return currentCoroutine;
    }

    private IEnumerator FadeRoutine(float target, float time)
    {
        while (!Mathf.Approximately(group.alpha, target))
        {
            group.alpha = Mathf.MoveTowards(group.alpha, target, Time.deltaTime / time);
            yield return null;
        }
    }
}
