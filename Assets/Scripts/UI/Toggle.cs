using UnityEngine;

public class Toggle : MonoBehaviour
{
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
}
