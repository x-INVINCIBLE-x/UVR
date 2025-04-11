using JetBrains.Annotations;
using System.Linq;
using UnityEngine;

public enum UIDirection
{
    North, 
    South,
    West,
    East,
    None
}

public class Ability_UI : MonoBehaviour
{
    public GameObject abilityDisplay;
    public Transform abiitySelectUIParent;
    public Transform[] abilitySelectDisplay;

    [Tooltip("0 -> North, 1 -> South, 2 -> West, 3 -> East")]
    public Action[] actions;
    private InputManager inputManager;
    private UIDirection uiSelect;
    private UIDirection lastSelected = UIDirection.None;

    private void Awake()
    {
        //abilitySelectDisplay = abiitySelectUIParent.GetComponentsInChildren<Transform>(true)
        //    .Skip(1)
        //    .ToArray();
    }

    private void Start()
    {
        inputManager = InputManager.Instance;
        inputManager.rightJoystickPress.action.performed += ToggleAbilitySelect;
        //HighlightSlot(Vector3.up);
    }

    private void ToggleAbilitySelect(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        abilityDisplay.SetActive(!abilityDisplay.activeSelf);

        if (abilityDisplay.activeSelf)
            inputManager.rightJoystick.action.performed += HandleJoysickMovement;
        else
        {
            inputManager.rightJoystick.action.performed -= HandleJoysickMovement;
            SelectAbility();
        }
    }

    public void ToggleAbilitySelect()
    {
        abilityDisplay.SetActive(!abilityDisplay.activeSelf);
    }

    private void HandleJoysickMovement(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        Vector3 direction = Vector3.up;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) 
        {
            uiSelect = input.x > 0 ? UIDirection.East : UIDirection.West;
            direction = Vector3.right;
        }
        else
        {
            uiSelect = input.y > 0 ? UIDirection.North : UIDirection.South;
            direction = Vector3.up;
        }

        if (uiSelect != lastSelected)
        {
            HighlightSlot(direction);
        }
    }

    private void HighlightSlot(Vector2 direction)
    {
        int multiplier = -1;

        if (uiSelect == UIDirection.North || uiSelect == UIDirection.East)
            multiplier = 1;

        if (lastSelected != UIDirection.None)
        {
            abilitySelectDisplay[((int)lastSelected)].GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            abilitySelectDisplay[((int)lastSelected)].localScale = Vector3.one;
        }

        abilitySelectDisplay[((int)uiSelect)].GetComponent<RectTransform>().anchoredPosition += (0.01f * multiplier * direction);
        abilitySelectDisplay[((int)uiSelect)].localScale = Vector3.one * 1.3f;

        lastSelected = uiSelect;
    }

    public void SelectAbility()
    {
        for (int i = 0; i < actions.Length; i++)
        {
            if (actions[i] != null)
                actions[i].PermitAbility(false);
        }
        
        if (actions[((int)uiSelect)] != null)
        {
            actions[((int)uiSelect)].PermitAbility(true);
            lastSelected = uiSelect;
        }
    }

    private void OnDisable()
    {
        inputManager.rightJoystickPress.action.performed -= ToggleAbilitySelect;
    }
}
