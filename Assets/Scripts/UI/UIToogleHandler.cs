using System;
using UnityEngine;

public class UIToogleHandler : MonoBehaviour
{
    public event System.Action OnClose;
    public bool closeOnMove = false;

    private void Start()
    {
        if (closeOnMove)
        {
            InputManager.Instance.leftJoystick.action.performed += Close;
        }
    }

    private void Close(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Close();
    }

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

    private void OnDestroy()
    {
        InputManager.Instance.leftJoystick.action.performed -= Close;
    }
}
