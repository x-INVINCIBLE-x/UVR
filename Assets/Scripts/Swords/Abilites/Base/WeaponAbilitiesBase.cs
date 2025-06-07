using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Audio;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class WeaponAbilitiesBase : MonoBehaviour
{
    
    public VelocityEstimator velocityEstimator;

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

    protected virtual void Start()
    {
        
        WeaponSFXSource = GetComponent<AudioSource>();
    }

    protected virtual void SlashAudio()
    {
        //SlashSFXSource.Stop(); // to stop ongoing sfx sounds
        //SlashSFXSource.clip = SlashSFX;
        //SlashSFXSource.loop = false;
        WeaponSFXSource.pitch = Random.Range(minPitch, maxPitch);
        WeaponSFXSource.PlayOneShot(WeaponSFX);
    }


}
