using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class ShootEnemy : MonoBehaviour
{
    private void OnEnable()
    {
        StopAllCoroutines();
        Rigidbody.linearVelocity = Vector3.zero; // Reset velocity
        Rigidbody.AddForce(transform.forward * Force, ForceMode.Impulse); // Apply force instantly
        StartCoroutine(DelayDisable());
    }

    private IEnumerator DelayDisable()
    {
        if (Wait == null)
        {
            Wait = new WaitForSeconds(AutoDestroyTime);
        }
        yield return Wait;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Rigidbody.angularVelocity = Vector3.zero;
        Rigidbody.linearVelocity = Vector3.zero;
    }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }

    [SerializeField] private float AutoDestroyTime = 1f;
    [SerializeField] private float Force = 20f; // Adjusted force

    private WaitForSeconds Wait;
    private Rigidbody Rigidbody;
}
