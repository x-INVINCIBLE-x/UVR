using System;
using Unity.VisualScripting;
using UnityEngine;

public class Action : MonoBehaviour
{
    protected ActionMediator actionMediator;
    protected InputManager inputManager;
    public bool isPermitted;

    [SerializeField] protected float skillDuration = 0f;

    [SerializeField] protected float skillCooldown;
    protected float lastTimeSkillUsed = -10f;
    public bool showDurationDisplay = false;
    
    protected virtual void Awake()
    {
        actionMediator = GetComponentInParent<ActionMediator>();
    }

    protected virtual void Start()
    {
        inputManager = InputManager.Instance;
        inputManager.YTap.action.performed += ctx => StartAbility();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            StartAbility();
    }

    public virtual void PermitAbility(bool status)
    {
        isPermitted = status;
    }

    protected virtual void StartAbility()
    {
        if (!CanUseAbility()) return;

        lastTimeSkillUsed = Time.time;
        ExecuteAbility();

        if (skillDuration > 0f && showDurationDisplay)
            UI.Instance.playerUI.StartAbilityDurationCooldown(skillDuration);
    }

    protected virtual void ExecuteAbility()
    {
        
    }

    protected virtual bool CanUseAbility()
    {
        if (!isPermitted || lastTimeSkillUsed + skillCooldown > Time.unscaledTime)
        {
            return false;
        }

        return true;
    }

    protected virtual void OnDestroy()
    {
        inputManager.YTap.action.performed -= ctx => StartAbility();
    }
}
