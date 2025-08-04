using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Rigidbody))]
public class HomingMissile : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Rigidbody _rb;
    //[SerializeField] private Target _target;
    [SerializeField] private GameObject _explosionPrefab;

    [Header("MOVEMENT")]
    [SerializeField] private float _speed = 15;
    [SerializeField] private float _acceleration = 1f;
    [SerializeField] private float _rotateSpeed = 95;

    [Header("PREDICTION")]
    [SerializeField] private float _maxDistancePredict = 100;
    [SerializeField] private float _minDistancePredict = 5;
    [SerializeField] private float _maxTimePrediction = 5;
    [SerializeField] private float _heightOffset = 1;
    private Vector3 _standardPrediction, _deviatedPrediction;
    private Vector3 offset;

    [Header("DEVIATION")]
    [SerializeField] private float _deviationAmount = 50;
    [SerializeField] private float _deviationSpeed = 2;

    [Header("Attack Info")]
    private LayerMask _targetLayer;
    private Rigidbody _targetRb;
    private AttackData _attackData;
    private float _attackAmount;
    private float _homingDuration;
    private float _lifeTime;
    private bool _isActive = false;

    private void Awake()
    {
        //Player = GameObject.Find("Cube");
        //_target = Player;
    }

    public void Setup(Rigidbody target, AttackData attackData, float speed, float acceleration, float homingDuration, float lifeTime, LayerMask tagetLayer = default)
    { 
        if (_rb == null)
            _rb = GetComponent<Rigidbody>();

        _targetLayer = tagetLayer;
        _targetRb = target;
        _attackData = attackData;
        _homingDuration = homingDuration;
        _lifeTime = lifeTime;
        _speed = speed;
        _acceleration = acceleration;
        _isActive = true;

        offset = new Vector3(0, _heightOffset, 0);
    }

    public void Setup(Rigidbody target, float attackAmount, float speed, float acceleration, float homingDuration, float lifeTime, LayerMask tagetLayer = default)
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody>();

        _targetLayer = tagetLayer;
        _targetRb = target;
        _attackAmount = attackAmount;
        _homingDuration = homingDuration;
        _lifeTime = lifeTime;
        _speed = speed;
        _acceleration = acceleration;
        _isActive = true;

        offset = new Vector3(0, _heightOffset, 0);
    }

    private void Update()
    {
        if (!_isActive) return;

        _lifeTime -= Time.deltaTime;
        _homingDuration -= Time.deltaTime;

        if (_lifeTime <= 0)
        {
            SelfDestruct();
        }
    }

    private void FixedUpdate()
    {
        if (_homingDuration < 0) return;

        //_speed += _acceleration * Time.fixedDeltaTime;

        _rb.linearVelocity = transform.forward * _speed;

        if (_targetRb == null || !_targetRb.gameObject.activeInHierarchy)
        {
            SelfDestruct();
            return;
        }
        var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, _targetRb.transform.position));

        PredictMovement(leadTimePercentage);

        AddDeviation(leadTimePercentage);

        RotateRocket();
    }

    private void PredictMovement(float leadTimePercentage)
    {
        var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);

        _standardPrediction = _targetRb.position + offset + _targetRb.linearVelocity * predictionTime;
    }

    private void AddDeviation(float leadTimePercentage)
    {
        var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);

        var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;

        _deviatedPrediction = _standardPrediction + predictionOffset;
    }

    private void RotateRocket()
    {
        var heading = _deviatedPrediction - transform.position;

        var rotation = Quaternion.LookRotation(heading);
        _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_targetLayer != default && (_targetLayer.value & (1 << collision.gameObject.layer)) == 0) return;
        if (_explosionPrefab) Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        if (collision.transform.TryGetComponent(out IAimable ex)) ex.Explode();

        IDamageable damagable = collision.transform.GetComponentInParent<IDamageable>();
        damagable ??= collision.transform.GetComponentInChildren<IDamageable>();

        damagable?.TakeDamage(_attackData);

        SelfDestruct();
    }

    private void SelfDestruct()
    {
        _isActive = false;
        ObjectPool.instance.ReturnObject(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _standardPrediction);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
    }
}


