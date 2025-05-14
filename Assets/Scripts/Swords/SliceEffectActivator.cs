using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SliceEffectActivator : MonoBehaviour
{

    public VelocityEstimator velocityEstimator;
    

    [Header("Activation Condition")]
    public bool SlashAttackAbility = false;
    public float minVelocity;
    public float minPitch = 1f;
    public float maxPitch = 2f;
    public GameObject SlashVFX;

    [Header("Audio")]
    public AudioClip SlashSFX;
    private AudioSource SlashSFXSource;


    private void Start()
    {
        //velocityEstimator = GetComponent<VelocityEstimator>();
        SlashSFXSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        ActivateSlashEffect();
    }
    private void ActivateSlashEffect()
    {
        float velocity = velocityEstimator.GetVelocityEstimate().magnitude;

        if (velocity > 3)
        {
            Debug.Log(velocity);
            //Instantiate(SlashVFX, gameObject.transform.position,Quaternion.identity);
            //Destroy(SlashVFX,1f);*
            GameObject SlashPool = ObjectPool.instance.GetObject(SlashVFX, transform);
            SlashAudio();
            
        }

        ObjectPool.instance.ReturnObject(SlashVFX);
    }

    private void SlashAbilityInputActiavtor()
    {
        // Add input from controller to activate the ability on and set SlashAttackAbility to true condition

        if (Input.GetKey(KeyCode.Z))
        {
            SlashAttackAbility = true;
        }
    }

    private void SlashAudio()
    {
        //SlashSFXSource.Stop(); // to stop ongoing sfx sounds
        //SlashSFXSource.clip = SlashSFX;
        //SlashSFXSource.loop = false;
        SlashSFXSource.pitch = Random.Range(minPitch, maxPitch);
        SlashSFXSource.PlayOneShot(SlashSFX);
    }



}
