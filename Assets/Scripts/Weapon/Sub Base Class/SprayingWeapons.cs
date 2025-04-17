public class SprayingWeapons : Weapon
{
    private void Start()
    {
        //InputManager.Instance.activate.action.performed += ctx => StartSpraying();
        //InputManager.Instance.activate.action.canceled += ctx => StopSpraying();
    }

    // Flamethrower , acid thrower and other spray type weapons
    protected virtual void StartSpraying()
    {

    }

    protected virtual void StopSpraying()
    {

    }

    private void OnDisable()
    {
        //InputManager.Instance.activate.action.performed -= ctx => StartSpraying();
        //InputManager.Instance.activate.action.canceled -= ctx => StopSpraying();
    }
}
