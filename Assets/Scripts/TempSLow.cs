using UnityEngine;

public class TempSLow : MonoBehaviour, ISlowable
{
    public bool isSLowed = false;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isSLowed)
        {
            rb.linearDamping = 5f; // Increase drag to slow movement gradually
            rb.angularDamping = 5f;
        }
    }

    public void OnSlowStart(float speedMultiplir)
    {
        isSLowed = true;
    }

    public void OnSlowStop()
    {
        isSLowed = false;
    }
}
