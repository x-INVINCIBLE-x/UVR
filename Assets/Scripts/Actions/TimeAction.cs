using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class TimeAction : Action
{
    private ActionMediator actionMediator;
    private InputManager inputManager;
    public DynamicMoveProvider dynamicMoveProvider;

    private float defaultMoveSpeed;

    private void Awake()
    {
        actionMediator = GetComponentInParent<ActionMediator>();
    }

    private void Start()
    {
        inputManager = InputManager.Instance;
        //inputManager.XTap.action.performed += ModifyTime;
    }

    protected override void ExecuteAbility()
    {
        base.ExecuteAbility();
        ModifyTime();
    }

    private void ModifyTime(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Time.timeScale = Time.timeScale == 1 ? 0.4f : 1f;
        AdjustSpeed();

        actionMediator.TimeScaleUpdated(Time.timeScale);
    }

    private void ModifyTime()
    {
        Time.timeScale = Time.timeScale == 1 ? 0.4f : 1f;
        AdjustSpeed();

        actionMediator.TimeScaleUpdated(Time.timeScale);
    }

    private void AdjustSpeed()
    {
        if (Time.timeScale != 1)
        {
            defaultMoveSpeed = dynamicMoveProvider.moveSpeed;
            dynamicMoveProvider.moveSpeed = defaultMoveSpeed * 2;
        }
        else
        {
            dynamicMoveProvider.moveSpeed = defaultMoveSpeed;
        }
    }
}
