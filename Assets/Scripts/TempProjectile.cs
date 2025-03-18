using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class TempProjectile : MonoBehaviour, ISlowable
{
    public float speed = 5f;
    private float defaultSpeed;

    private void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().linearVelocity = Vector3.forward * speed;
    }

    public void OnSlowStart(float slowMultiplier)
    {
        defaultSpeed = speed;
        speed = defaultSpeed * slowMultiplier;
    }

    public void OnSlowStop()
    {
        speed = defaultSpeed;
    }
}
