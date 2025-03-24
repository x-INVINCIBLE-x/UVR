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

    public void Close(GameObject element)
    {
        element.SetActive(false);
    }
}
