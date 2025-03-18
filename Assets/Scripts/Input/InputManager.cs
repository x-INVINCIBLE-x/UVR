using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public InputActionProperty leftJoystickPress;
    public InputActionProperty rightJoystickPress;
    public InputActionProperty X;
    public InputActionProperty Y;
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
