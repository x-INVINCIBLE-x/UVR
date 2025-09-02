using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;

    [Header("Beam Size Settings")]
    [SerializeField] private float initialWidth;
    [SerializeField] private float finalWidth;
    [SerializeField] private float initialHeight;
    [SerializeField] private float maxHeight;
    [SerializeField] private float heightIncreaseSpeed;

    [Header("Beam Timing Settings")]
    private float warningDuration;
    private float activeDuration;
    [SerializeField] private float damageRate = 0.2f;

    [SerializeField] private DamageOnTouch damageProvider;
    private bool canIncreaseHeight = true;
    private LayerMask groundLayer;

    public void Setup(float _warningDuration, float _activeDuration, AttackData _attackData)
    {
        warningDuration = _warningDuration;
        activeDuration = _activeDuration;

        damageProvider.Setup(_attackData, damageRate, targetLayer);

        groundLayer = LayerMask.GetMask("Ground");

        StartCoroutine(BeamRoutine());
    }

    private void Update()
    {
        if (canIncreaseHeight && transform.localScale.y < maxHeight)
        {
            float newHeight = transform.localScale.y + heightIncreaseSpeed * Time.deltaTime;
            transform.localScale = new Vector3(transform.localScale.x, newHeight, transform.localScale.z);
        }
    }

    IEnumerator BeamRoutine()
    {
        float timer = 0f;
        float width;

        // Initialize beam
        while (timer < warningDuration)
        {
            timer += Time.deltaTime;
            width = Mathf.Lerp(initialWidth, 0f, timer / warningDuration);
            //height = Mathf.Lerp(initialHeight, finalHeight, timer / warningDuration);
            transform.localScale = new Vector3(width, transform.localScale.y, width);
            yield return null;
        }

        // Activate beam
        damageProvider.enabled = true;
        timer = 0f;
        while (timer < activeDuration)
        {
            timer += Time.deltaTime;
            width = Mathf.Lerp(0f, finalWidth, timer / activeDuration);
            transform.localScale = new Vector3(width, transform.localScale.y, width);
            yield return null;
        }

        // Deactivate beam
        damageProvider.enabled = false;
        ObjectPool.instance.ReturnObject(gameObject);
    }

    private void StopHeightIncrease()
    {
        canIncreaseHeight = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canIncreaseHeight) return;
        if (((1 << other.gameObject.layer) & groundLayer) == 0) return;

        Invoke(nameof(StopHeightIncrease), 0.5f);
    }
}
