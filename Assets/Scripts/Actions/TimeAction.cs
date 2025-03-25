using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class TimeAction : Action
{
    public DynamicMoveProvider dynamicMoveProvider;
    public ContinuousTurnProvider continuousTurnProvider;

    [SerializeField] private float slowDuration = 5f;
    private float slowTimer = 0f;
    private float defaultMoveSpeed;
    private float defaultTurnSpeed;

    private Coroutine timerRoutine = null;

    protected override void Start()
    {
        base.Start();
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

        if (Time.timeScale != 1)
        {
            slowTimer = slowDuration;
            if (timerRoutine == null)
                StartCoroutine(TimeSlowDurationRoutine());
        }

        AdjustSpeed();
        actionMediator.TimeScaleUpdated(Time.timeScale);
    }

    private void ModifyTime()
    {
        Time.timeScale = Time.timeScale == 1 ? 0.4f : 1f;

        if (Time.timeScale != 1)
        {
            slowTimer = slowDuration;
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

            dynamicMoveProvider.moveSpeed = defaultMoveSpeed * (1 / Time.timeScale);
            continuousTurnProvider.turnSpeed = defaultTurnSpeed * (1 / Time.timeScale);
        }
        else
        {
            dynamicMoveProvider.moveSpeed = defaultMoveSpeed;
            continuousTurnProvider.turnSpeed = defaultTurnSpeed;
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
}
