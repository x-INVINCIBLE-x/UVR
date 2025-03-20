using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public InputActionProperty leftJoystick;
    public InputActionProperty leftJoystickPress;
    public InputActionProperty rightJoystickPress;
    public InputActionProperty XTap;
    public InputActionProperty XDoubleTap;
    public InputActionProperty XHold;
    public InputActionProperty YTap;
    public InputActionProperty YDoubleTap;
    public InputActionProperty YHold;
    public InputActionProperty A;
    public InputActionProperty B;

    private void Awake()
    {
        if (Instance == null)
            Instance = this; 
        else
            Destroy(gameObject);
    }
}
