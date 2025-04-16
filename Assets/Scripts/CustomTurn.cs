using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class CustomTurn : MonoBehaviour
{
    public ContinuousTurnProvider xrTurnProvider;
    public Transform xrOriginTransform;
    public float customTurnSpeed = 60f;
    public bool useCustomTurning = false;

    public InputActionProperty turnInputAction;

    void Update()
    {
        if (turnInputAction == null || xrOriginTransform == null) return;

        if (useCustomTurning)
        {
            if (xrTurnProvider.enabled)
                xrTurnProvider.enabled = false;

            float input = turnInputAction.action.ReadValue<Vector2>().x;
            RotatePlayer(input);
        }
        else
        {
            if (!xrTurnProvider.enabled)
                xrTurnProvider.enabled = true;
        }
    }

    void RotatePlayer(float input)
    {
        if (Mathf.Abs(input) > 0.01f)
        {
            float angle = input * customTurnSpeed * Time.unscaledDeltaTime;
            xrOriginTransform.Rotate(0f, angle, 0f);
        }
    }
}