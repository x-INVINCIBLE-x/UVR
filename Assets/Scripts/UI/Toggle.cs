using System.Collections;
using UnityEngine;

public class Toggle : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string openAnimationName;
    [SerializeField] private string closeAnimationName;

    public void ToggleObject(GameObject element)
    {
        if (element == null)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
        else
        {

            element.SetActive(!element.activeSelf);
        }
    }

    public void AnimatedToggle()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            if (openAnimationName != null)
                animator.Play(openAnimationName);
        }
        else
        {
            if (closeAnimationName == null)
                gameObject.SetActive(false);
            else
            {
                animator.Play(closeAnimationName);
                StartCoroutine(WaitForAnimation(closeAnimationName));
            }
        }
    }

    public void ToggleWithDelay(float delay)
    {
        if (!gameObject.activeSelf)
            Invoke(nameof(SetActive), delay);
        else
            gameObject.SetActive(false);
    }

    private void SetActive()
    {
        gameObject.SetActive(true);
    }

    public void Close(GameObject element)
    {
        element.SetActive(false);
    }

    private IEnumerator WaitForAnimation(string animationName)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f )
            yield return null;

        gameObject.SetActive(false);
    }
}
