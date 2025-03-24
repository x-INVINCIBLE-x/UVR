using System;
using Unity.VisualScripting;
using UnityEngine;

public class Action : MonoBehaviour
{
    protected ActionMediator actionMediator;
    protected InputManager inputManager;
    public bool isPermitted;

    [SerializeField] protected float skillCooldown;
    private float lastTimeSkillUsed = -10f;
    
    protected virtual void Awake()
    {
        actionMediator = GetComponentInParent<ActionMediator>();
    }

    protected virtual void Start()
    {
        inputManager = InputManager.Instance;
        inputManager.leftJoystickPress.action.performed += ctx => StartAbility();
    }

    private void StartAbility()
    {
        if (!CanUseAbility()) return;

        lastTimeSkillUsed = Time.time;
        ExecuteAbility();
    }

    protected virtual void ExecuteAbility()
    {
        
    }

    private bool CanUseAbility()
    {
        if (!isPermitted || lastTimeSkillUsed + skillCooldown > Time.time)
        {
            return false;
        }

        return true;
    }

    protected virtual void OnDestroy()
    {
        inputManager.leftJoystickPress.action.performed -= ctx => StartAbility();
    }
}
