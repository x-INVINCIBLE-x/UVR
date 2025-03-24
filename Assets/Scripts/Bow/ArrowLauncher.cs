using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class ArrowLauncher : MonoBehaviour
{
    [Header("Launch Settings")]
    [SerializeField] private float _speed = 10f;

    [Header("Visual Effects")]
    //[SerializeField] private GameObject _trailSystem;

    private Rigidbody _rigidbody;
    private bool _inAir = false;
    private XRPullInteractable _pullInteractable;

    private void Awake()
    {
        InitializeComponents();
        SetPhysics(false);
    }

    private void InitializeComponents()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError($"Rigidbody component not found on Arrow {gameObject.name}");
        }
    }

    public void Initialize(XRPullInteractable pullInteractable)
    {
        _pullInteractable = pullInteractable;
        _pullInteractable.PullActionReleased += Release;
    }

    private void OnDestroy()
    {
        if (_pullInteractable != null)
        {
            _pullInteractable.PullActionReleased -= Release;
        }
    }

    private void Release(float value)
    {
        if (_pullInteractable != null)
        {
            _pullInteractable.PullActionReleased -= Release;
        }

        gameObject.transform.parent = null;
        _inAir = true;
        SetPhysics(true);

        Vector3 force = transform.forward * value * _speed;
        _rigidbody.AddForce(force, ForceMode.Impulse);

        StartCoroutine(RotateWithVelocity());
        //_trailSystem.SetActive(true);

    }


    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();
        while (_inAir)
        {
            if (_rigidbody != null && _rigidbody.linearVelocity.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(_rigidbody.linearVelocity, transform.up);
            }
            yield return null;
        }
    }


    public void StopFlight()
    {
        _inAir = false;
        SetPhysics(false);
        //_trailSystem.SetActive(false);
    }

    private void SetPhysics(bool usePhysics)
    {
        if (_rigidbody != null)
        {
            _rigidbody.useGravity = usePhysics;
            _rigidbody.isKinematic = !usePhysics;
        }
    }
}
