using System;
using UnityEngine;

public class UIToogleHandler : MonoBehaviour
{
    public event System.Action OnClose;

    public void Toggle()
    {
        if (gameObject.activeSelf)
        {
            Close();
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void Close()
    {
        if (!gameObject.activeSelf) return;

        OnClose?.Invoke();
        gameObject.SetActive(false);
    }
}
