using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class WeaponAbilitiesBase : MonoBehaviour
{

    [Space]
    [Header("CoolDown Settings")]
    [Space]
    [SerializeField] protected float fireRate = 3f;
    [SerializeField] protected float maxHeatLimit = 100f;
    [SerializeField] protected float coolDownRate = 5f;
    [SerializeField] protected float overheatResistance = 6f;
    [SerializeField] protected float currentHeat = 0f;
    [SerializeField] protected bool isOverheated = false;
    protected Coroutine cooldownCoroutine;

    public VelocityEstimator velocityEstimator;
    private XRGrabInteractable interactableWeapon;
    protected Rigidbody rigidBody;

    private Vector3? contactPoint = null;
    private Vector3? contactNormal = null;


    [Header("Activation Condition")]
    [Space]
    [SerializeField] protected bool AbilityEnable = false;
    [SerializeField] protected float minPitch = 1f;
    [SerializeField] protected float maxPitch = 2f;
    [SerializeField] protected GameObject WeaponVFX;

    [Space]
    [Header("Audio")]
    public AudioClip WeaponSFX;
    private AudioSource WeaponSFXSource;

    protected virtual void Awake()
    {
        interactableWeapon = GetComponent<XRGrabInteractable>();
        rigidBody = GetComponent<Rigidbody>();
        WeaponSFXSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        interactableWeapon.selectEntered.AddListener(PickUpWeapon);
        interactableWeapon.selectExited.AddListener(DropWeapon);
    }

    private void PickUpWeapon(SelectEnterEventArgs arg0)
    {
        interactableWeapon.activated.AddListener(ActivateWeapon);
        interactableWeapon.deactivated.AddListener(DeactivateWeapon);
    }

    private void DropWeapon(SelectExitEventArgs arg0)
    {
        DeactivateWeapon(default);

        interactableWeapon.activated.RemoveListener(ActivateWeapon);
        interactableWeapon.deactivated.RemoveListener(DeactivateWeapon);
    }

    protected virtual void AllAttacks()
    {
        if (!CanAttack()) return;
    }

    protected virtual bool CanAttack()
    {
        return !isOverheated; // Return true if not overheated
    }

    protected virtual void SlashAudio()
    {
        WeaponSFXSource.pitch = Random.Range(minPitch, maxPitch);
        WeaponSFXSource.PlayOneShot(WeaponSFX);
    }

    protected virtual void ActivateWeapon(ActivateEventArgs arg)
    {
        AbilityEnable = true;        
    }

    protected virtual void DeactivateWeapon(DeactivateEventArgs arg)
    {
        AbilityEnable = false;
    }

    protected virtual IEnumerator ReduceValueOverTime()
    {
        while (currentHeat > 0 && !isOverheated)
        {
            currentHeat -= overheatResistance * Time.deltaTime;
            currentHeat = Mathf.Clamp(currentHeat, 0, maxHeatLimit);
            yield return null;
        }
    }

    protected virtual IEnumerator CoolDownPhase()
    {
        while (currentHeat > 0)
        {
            currentHeat -= coolDownRate * Time.deltaTime;
            currentHeat = Mathf.Clamp(currentHeat, 0, maxHeatLimit);
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
    }
}
