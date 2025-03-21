using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class TimeAction : Action
{
    public DynamicMoveProvider dynamicMoveProvider;

    [SerializeField] private float slowDuration = 5f;
    private float slowTimer = 0f;
    private float defaultMoveSpeed;
    

    protected override void Start()
    {
        base.Start();
        inputManager.XTap.action.performed += ModifyTime;
    }

    protected override void ExecuteAbility()
    {
        base.ExecuteAbility();
        ModifyTime();
    }

    private void ModifyTime(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Time.timeScale = Time.timeScale == 1 ? 0.4f : 1f;

        if (Time.timeScale != 1)
            slowTimer = Time.time;

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
            dynamicMoveProvider.moveSpeed = defaultMoveSpeed * (1/Time.timeScale);
        }
        else
        {
            dynamicMoveProvider.moveSpeed = defaultMoveSpeed;
        }
    }
}
