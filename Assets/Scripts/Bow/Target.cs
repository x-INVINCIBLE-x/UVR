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

   //fgdfgdf
    void Update()
    {
        float randSize = Random.Range(0, _size);
        float randSpeed = Random.Range(0, _speed);
        var dir = new Vector3(Mathf.Cos(Time.time * randSpeed) * randSize, Mathf.Sin(Time.time * randSpeed) * randSize);

        _rb.linearVelocity = dir;
    }

    void setRandomValue()
    {

    }
    public void Explode() => Destroy(gameObject);
}

