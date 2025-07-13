using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class ScatterWeapons : Weapon
{
    [Header("Shotgun Settings")]
    [SerializeField] protected List<Transform> bulletSpawns;
    [SerializeField] protected float shootingForce;
    [SerializeField] protected float recoilForce;
    [SerializeField] protected float damage;
    [SerializeField] protected float spreadAngle = 5f; // Spread angle in degrees
    [SerializeField] protected GameObject muzzleVFX;

    protected AudioSource WeaponAudioSource;


    [Header("CoolDown")]
    [Space]
    [SerializeField] protected float fireRate = 3f;
    [SerializeField] protected float maxHeatLimit = 100f;
    [SerializeField] protected float coolDownRate = 5f;
    [SerializeField] protected float overheatResistance = 6f;
    [SerializeField] protected float currentHeat = 0f;
    [SerializeField] protected bool isOverheated = false;

    protected Coroutine cooldownCoroutine;

    [Header("CooldownShader Settings")]
    [Space]
    [SerializeField] protected Material OverHeatMaterial;
    [SerializeField] protected float currentEmissionIntensity = 0f;
    [SerializeField] protected float minEmissionIntensity = 0.02f; // The less the value the lesser the final intensity of weapon
    private float maxEmissionIntensity; // max brightness (original brightness)
    private UnityEngine.Color originalEmissionColor;
    private static readonly int EmissionColorID = Shader.PropertyToID("_Color"); // shader referenece 


    protected override void Awake()
    {
        base.Awake();
        WeaponAudioSource = GetComponent<AudioSource>();

        if (OverHeatMaterial != null)
        {
            originalEmissionColor = OverHeatMaterial.GetColor(EmissionColorID);

            // Optional: derive brightness from color
            float baseIntensity = (originalEmissionColor.r + originalEmissionColor.g + originalEmissionColor.b) / 3f;
            maxEmissionIntensity = baseIntensity * 1.5f; // Boost brightness 

            //Debug.Log($"Original Emission Color: {originalEmissionColor}, Max Intensity: {maxEmissionIntensity}");
        }
    }

    private void Start()
    {
        InputManager.Instance.activate.action.performed += ctx => ScatterShot();

    }
    protected virtual void ScatterShot()
    {
        if (!CanShoot()) return;

        ApplyHeat();
        ApplyRecoil();
    }

    protected bool CanShoot()
    {
        return !isOverheated;
    }

    private void ApplyRecoil()
    {
        rigidBody.AddRelativeForce(Vector3.back * recoilForce, ForceMode.Impulse);
    }


    protected virtual IEnumerator ReduceValueOverTime()
    {
        while (currentHeat > 0 && !isOverheated)
        {
            currentHeat -= overheatResistance * Time.deltaTime;
            currentHeat = Mathf.Clamp(currentHeat, 0, maxHeatLimit);
            UpdateEmissionBasedOnHeat();
            yield return null;
        }
    }

    protected virtual IEnumerator CoolDownPhase()
    {
        while (currentHeat > 0)
        {
            currentHeat -= coolDownRate * Time.deltaTime;
            currentHeat = Mathf.Clamp(currentHeat, 0, maxHeatLimit);
            UpdateEmissionBasedOnHeat();
            yield return null;
        }

        isOverheated = false;
        cooldownCoroutine = null;

        if (currentHeat > 0)
        {
            cooldownCoroutine = StartCoroutine(ReduceValueOverTime());
        }
    }

    protected virtual void ApplyHeat()
    {
        currentHeat += maxHeatLimit / fireRate;
        currentHeat = Mathf.Clamp(currentHeat, 0, maxHeatLimit);
        UpdateEmissionBasedOnHeat();

        if (currentHeat >= maxHeatLimit)
        {
            Overheat();
        }
        else if (!isOverheated)
        {
            cooldownCoroutine = StartCoroutine(ReduceValueOverTime());
        }
    }

    protected virtual void Overheat()
    {
        isOverheated = true;

        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }

        cooldownCoroutine = StartCoroutine(CoolDownPhase());
    }

    private void OnDisable()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }

        OverHeatMaterial.SetColor(EmissionColorID, originalEmissionColor);
    }

    private void UpdateEmissionBasedOnHeat() // Extra adjustment refinement , checks based on current heat value
    {
        float t = 1f - (currentHeat / maxHeatLimit);
        t = Mathf.Pow(t, 2f); // Exponential dimming
        currentEmissionIntensity = Mathf.Lerp(minEmissionIntensity, maxEmissionIntensity, t);
        UpdateEmission(currentEmissionIntensity);
    }

    private void UpdateEmission(float strength)
    {
        var emissionColor = originalEmissionColor * strength;
        OverHeatMaterial.SetColor(EmissionColorID, emissionColor);
        //Debug.Log($"[Heat: {currentHeat:F1}] Emission RGB: {emissionColor.r:F2}, {emissionColor.g:F2}, {emissionColor.b:F2}");
    }
}
