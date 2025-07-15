using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class WeaponAbilitiesBase : MonoBehaviour
{
    
    public VelocityEstimator velocityEstimator;
    private XRGrabInteractable interactableWeapon;
    protected Rigidbody rigidBody;

    private Vector3? contactPoint = null;
    private Vector3? contactNormal = null;


    [Header("Activation Condition")]
    [Space]
    [SerializeField] protected bool AbilityEnable = false;
    protected float minVelocity;
    protected float minPitch = 1f;
    protected float maxPitch = 2f;
    
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

    protected virtual void Start()
    {
        
        
    }

    protected virtual void SlashAudio()
    {
        //SlashSFXSource.Stop(); // to stop ongoing sfx sounds
        //SlashSFXSource.clip = SlashSFX;
        //SlashSFXSource.loop = false;
        WeaponSFXSource.pitch = Random.Range(minPitch, maxPitch);
        WeaponSFXSource.PlayOneShot(WeaponSFX);
    }


    protected virtual void SetupInteractableWeaponEvents()
    {
        interactableWeapon.activated.AddListener(ActivateWeapon);
        interactableWeapon.deactivated.AddListener(DeactivateWeapon);
    }
    protected virtual void OnDestroy()
    {
        interactableWeapon.activated.RemoveListener(ActivateWeapon);
        interactableWeapon.deactivated.RemoveListener(DeactivateWeapon);
    }

    protected virtual void ActivateWeapon(ActivateEventArgs args)
    {
        AbilityEnable = true;
    }
    protected virtual void DeactivateWeapon(DeactivateEventArgs args)
    {
        AbilityEnable = false;
    }

}
