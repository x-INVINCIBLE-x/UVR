using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Target : MonoBehaviour, IAimable
{
    
    private Rigidbody _rb;
    [SerializeField] private float _size = 10;
    [SerializeField] private float _speed = 10;
    public Rigidbody Rb => _rb;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        var dir = new Vector3(Mathf.Cos(Time.time * _speed) * _size, Mathf.Sin(Time.time * _speed) * _size);

        _rb.linearVelocity = dir;
    }

    public void Explode() => Destroy(gameObject);
}

