using UnityEngine;

public class TempSLow : MonoBehaviour, ISlowable
{
    public bool isSlowed = false;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isSlowed)
        {
            rb.linearDamping = 5f; // Increase drag to slow movement gradually
            rb.angularDamping = 5f;
        }
    }

    public void OnSlowStart(float speedMultiplir)
    {
        isSlowed = true;
    }

    public void OnSlowStop()
    {
        isSlowed = false;
    }
}
