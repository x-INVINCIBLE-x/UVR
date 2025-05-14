using UnityEngine;
using UnityEngine.UIElements;

public class SlashLauncher : MonoBehaviour
{
    private Rigidbody slashBody;
    public float force = 1000f;

    private void Awake()
    {
        slashBody = GetComponent<Rigidbody>();
        LaunchSlash();
    }
    public void LaunchSlash()
    {
        slashBody.linearVelocity = gameObject.transform.forward * force;
    }
}
