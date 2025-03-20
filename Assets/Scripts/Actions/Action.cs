using System;
using UnityEngine;

public class Action : MonoBehaviour
{
    public bool isPermitted;

    private void Start()
    {
        InputManager.Instance.rightJoystickPress.action.performed += ctx => StartAbility();
    }

    private void StartAbility()
    {
        if (!isPermitted) return;

        ExecuteAbility();
    }

    protected virtual void ExecuteAbility()
    {
        
    }

    private void OnDestroy()
    {
        InputManager.Instance.rightJoystickPress.action.performed -= ctx => StartAbility();
    }
}
