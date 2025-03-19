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
        actionMediator.TimeScaleUpdated(Time.timeScale);
    }

    private void Update()
    {
    }
}
