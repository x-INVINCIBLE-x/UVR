using System.Linq;
using UnityEngine;

public class Ability_UI : MonoBehaviour
{
    public GameObject abilityDisplay;
    public GameObject[] abilitySelectDisplay;

    public Action[] actions;
    private void Awake()
    {
        abilitySelectDisplay = GetComponentsInChildren<Transform>(true)
            .Skip(1)
            .Select(t => t.gameObject)
            .ToArray();
    }

    private void OnEnable()
    {
        InputManager.Instance.leftJoystick.action.performed += HandleJoysickMovement;
    }

    public void ToggleAbilitySelect()
    {
        abilityDisplay.SetActive(!abilityDisplay.activeSelf);
    }

    private void HandleJoysickMovement(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) 
        {
            SelectAbility(input.x > 0 ? "East" : "West");
        }
        else
        {
            SelectAbility(input.y > 0 ? "North" : "South");
        }
    }


    public void SelectAbility(string direction)
    {
        switch (direction)
        {
            case "North":
                actions[1].isPermitted = false;
                actions[0].isPermitted = true;
                break;
            case "South":
                actions[0].isPermitted = false;
                actions[1].isPermitted = true;
                break;
            case "East":
                break;
            case "West":
                break;
        }
    }

    private void OnDisable()
    {
        InputManager.Instance.leftJoystick.action.performed -= HandleJoysickMovement;
    }
}
