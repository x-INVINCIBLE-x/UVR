using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    None,
    Explode,
    Freeze
}

public enum LinkButton
{
    X, Y
}

public class AbilityHandler : MonoBehaviour
{
    public static AbilityHandler Instance;

    public Ability ActiveAbility {  get; private set; }
    public Ability XAbility;
    public Ability YAbility;

    public Transform scannerTransform;
    public float scannerRange;
    public float abilityRemoveDuration = 1.5f;
    public LayerMask whatIsScannable;

    private InputManager inputManager;
    private Coroutine abilityRemoveRoutine = null;
    private Dictionary<LinkButton, Ability> abilityLink;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitilalizeInput();

        abilityLink = new Dictionary<LinkButton, Ability>
        {
            { LinkButton.X, XAbility },
            { LinkButton.Y, YAbility }
        };
    }

    private void InitilalizeInput()
    {
        inputManager = InputManager.Instance;
        inputManager.XTap.action.performed += ctx => SwitchAbility(LinkButton.X);
        inputManager.YTap.action.performed += ctx => SwitchAbility(LinkButton.Y);

        inputManager.XDoubleTap.action.performed += ctx => ScanAbility(LinkButton.X);
        inputManager.YDoubleTap.action.performed += ctx => ScanAbility(LinkButton.Y);

        inputManager.XHold.action.started += ctx => RemoveAbility(LinkButton.X, ctx.phase);
        inputManager.XHold.action.canceled += ctx => RemoveAbility(LinkButton.X, ctx.phase);

        inputManager.YHold.action.started += ctx => RemoveAbility(LinkButton.Y, ctx.phase);
        inputManager.YHold.action.canceled += ctx => RemoveAbility(LinkButton.Y, ctx.phase);
    }

    private void ScanAbility(LinkButton button)
    {
        Debug.Log("Scan");
        if (Physics.SphereCast(scannerTransform.position, 1f, scannerTransform.forward, out RaycastHit hit, scannerRange, whatIsScannable))
        {
            if (hit.transform.TryGetComponent(out Scannable scannable))
            {
                hit.transform.localScale = hit.transform.localScale + Vector3.one;
                abilityLink[button].Type = scannable.Ability.Type;
                abilityLink[button].level = scannable.Ability.level;
            }
        }
    }

    private void SwitchAbility(LinkButton button) => ActiveAbility = abilityLink[button];

    private void RemoveAbility(LinkButton button, UnityEngine.InputSystem.InputActionPhase phase)
    {
        Debug.Log("Remove" + phase);
        switch (phase)
        {
            case UnityEngine.InputSystem.InputActionPhase.Started:
                if (abilityRemoveRoutine != null) return;
                abilityRemoveRoutine = StartCoroutine(RemoveAbilityRoutine(button));
                break;

            case UnityEngine.InputSystem.InputActionPhase.Canceled:
                if (abilityRemoveRoutine != null)
                {
                    StopCoroutine(abilityRemoveRoutine);
                    abilityRemoveRoutine = null;
                }
                break;
        }
    }

    private IEnumerator RemoveAbilityRoutine(LinkButton button)
    {
        yield return new WaitForSeconds(abilityRemoveDuration);

        abilityLink[button].Type = AbilityType.None;

        abilityRemoveRoutine = null;
    }

    private void OnDestroy()
    {
        inputManager = InputManager.Instance;
        inputManager.XTap.action.performed -= ctx => SwitchAbility(LinkButton.X);
        inputManager.YTap.action.performed -= ctx => SwitchAbility(LinkButton.Y);

        inputManager.XDoubleTap.action.performed -= ctx => ScanAbility(LinkButton.X);
        inputManager.YDoubleTap.action.performed -= ctx => ScanAbility(LinkButton.Y);

        inputManager.XHold.action.started -= ctx => RemoveAbility(LinkButton.X, ctx.phase);
        inputManager.XHold.action.canceled -= ctx => RemoveAbility(LinkButton.X, ctx.phase);

        inputManager.YHold.action.started -= ctx => RemoveAbility(LinkButton.Y, ctx.phase);
        inputManager.YHold.action.canceled -= ctx => RemoveAbility(LinkButton.Y, ctx.phase);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(scannerTransform.position, scannerTransform.forward * scannerRange);
    }
}

[System.Serializable]
public class Ability
{
    public AbilityType Type;
    [Range(1, 3)]
    public int level = 1;
}
