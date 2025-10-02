using System.Collections;
using UnityEngine;

public class OnSliceEventDestroy : MonoBehaviour
{
    [SerializeField] private ObjectiveType type;
    [SerializeField] private float shrinkDuration = 0.5f; 

    private void Start()
    {
        GameEvents.OnElimination += HandleElimination;
    }

    private void HandleElimination(ObjectiveType eliminatedType)
    {
        if (eliminatedType == type)
        {
            GameEvents.OnElimination -= HandleElimination;
            StartCoroutine(ShrinkAndDestroy());
        }
    }

    private IEnumerator ShrinkAndDestroy()
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = new Vector3(startScale.x, 0f, startScale.z);

        float elapsed = 0f;
        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkDuration;

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}
