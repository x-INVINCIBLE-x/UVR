using System.Collections;
using TMPro;
using UnityEngine;

public class KnowledgeProvider : MonoBehaviour, ISaveable
{
    [SerializeField] private TextMeshProUGUI knowledgeText;
    [SerializeField, TextArea] private string[] importantInfo;

    [SerializeField] private float delayBeforeStart = 2f;
    [SerializeField] private float showDuration = 3f;    
    [SerializeField] private float fadeDuration = 1f;

    private bool hasShown = false;
    
    private void Start()
    {
        if (!hasShown)
        {
            knowledgeText.text = "";
            StartCoroutine(ShowInfoSequence());
        }
    }

    private IEnumerator ShowInfoSequence()
    {
        PrioritySceneGate.Instance.MarkUnready(); 

        yield return new WaitForSeconds(delayBeforeStart);

        for (int i = 0; i < importantInfo.Length; i++)
        {
            yield return StartCoroutine(AnimateText(importantInfo[i]));
        }

        PrioritySceneGate.Instance.MarkReady();
    }

    private IEnumerator AnimateText(string text)
    {
        knowledgeText.text = text;
        knowledgeText.alpha = 0f;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            knowledgeText.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        knowledgeText.alpha = 1f;

        yield return new WaitForSeconds(showDuration);

        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            knowledgeText.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        knowledgeText.alpha = 0f;
    }


    public object CaptureState()
    {
        return hasShown;
    }

    public void RestoreState(object state)
    {
        hasShown = (bool)state;
    }
}
