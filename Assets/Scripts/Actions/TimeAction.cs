using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TimeAction : MonoBehaviour
{
    private ActionMediator actionMediator;
    private InputManager inputManager;

    private void Awake()
    {
        actionMediator = GetComponentInParent<ActionMediator>();
    }

    private void Start()
    {
        inputManager = InputManager.Instance;
        inputManager.Y.action.performed += ModifyTime;
    }

    private void ModifyTime(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Time.timeScale = Time.timeScale == 1 ? 0.4f : 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Time.timeScale = 0.3f;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            Time.timeScale = 1f;
        }
    }

    public void ModifyTime()
    {
        Time.timeScale = 1.0f;
    }

    
}
