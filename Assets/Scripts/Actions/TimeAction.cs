using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class TimeAction : Action
{
    public DynamicMoveProvider dynamicMoveProvider;
    public ContinuousTurnProvider continuousTurnProvider;

    private float slowTimer = 0f;
    private float defaultMoveSpeed;
    private float defaultTurnSpeed;
    private float defaultRbTorque;
    private float defaultRbMaxAngularVelocity;
    public bool Active;

    private Coroutine timerRoutine = null;

    protected override void Start()
    {
        base.Start();
        if (inputManager != null)
            inputManager.YTap.action.performed += ctx => HandleAbility();
        //inputManager.leftJoystickPress.action.performed += EndAbility;
    }

    private void HandleAbility()
    {
        if (CanUseAbility())
            StartAbility(); 
    }

    protected override void ExecuteAbility()
    {
        base.ExecuteAbility();
        ModifyTime();
    }

    private void EndAbility(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!isPermitted || Time.timeScale == 1) { return; }
        Time.timeScale = 1f;

        AdjustSpeed();
        actionMediator.TimeScaleUpdated(Time.timeScale);
        UI.Instance.playerUI.StopAbilityDurationCooldown();
    }

    private void ModifyTime()
    {
        Time.timeScale = Time.timeScale == 1 ? 0.4f : 1f;

        if (Time.timeScale != 1)
        {
            slowTimer = skillDuration;
            if (timerRoutine == null)
                StartCoroutine(TimeSlowDurationRoutine());
        }
        else if (timerRoutine != null)
            StopCoroutine(timerRoutine);

        AdjustSpeed();
        actionMediator.TimeScaleUpdated(Time.timeScale);
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1;
        AdjustSpeed();
        actionMediator.TimeScaleUpdated(Time.timeScale);
    }

    private void AdjustSpeed()
    {
        if (Time.timeScale != 1)
        {
            defaultMoveSpeed = dynamicMoveProvider.moveSpeed;
            defaultTurnSpeed = continuousTurnProvider.turnSpeed;
            defaultRbTorque = actionMediator.rbTurnProvider.torqueStrength;
            defaultRbMaxAngularVelocity = actionMediator.rbTurnProvider.maxAngularVelocity;

            dynamicMoveProvider.moveSpeed = defaultMoveSpeed * (1 / Time.timeScale);
            continuousTurnProvider.turnSpeed = defaultTurnSpeed * (1 / Time.timeScale);
            actionMediator.rbTurnProvider.torqueStrength = defaultRbTorque * (1 / Time.timeScale);
            actionMediator.rbTurnProvider.maxAngularVelocity = defaultRbMaxAngularVelocity * (1 / Time.timeScale);
        }
        else
        {
            dynamicMoveProvider.moveSpeed = defaultMoveSpeed;
            continuousTurnProvider.turnSpeed = defaultTurnSpeed;
            actionMediator.rbTurnProvider.torqueStrength = defaultRbTorque;
            actionMediator.rbTurnProvider.maxAngularVelocity = defaultRbMaxAngularVelocity;
        }
    }

    IEnumerator TimeSlowDurationRoutine()
    {
        while (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime * 1 / Time.timeScale;
            yield return new WaitForEndOfFrame();
        }

        ResetTimeScale();
        timerRoutine = null;
    }

    protected override void OnDestroy()
    {
        if (inputManager != null)
            inputManager.YTap.action.performed += ctx => HandleAbility();
    } 
}
