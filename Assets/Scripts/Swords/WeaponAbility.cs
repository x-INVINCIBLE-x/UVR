

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class WeaponAbility : MonoBehaviour
{

    public VelocityEstimator velocityEstimator;
    
    private Vector3? contactPoint = null;
    private Vector3? contactNormal = null;

    [Header("Activation Condition")]
    public bool SlashAttackAbility = false;
    public float minVelocity;
    public float minPitch = 1f;
    public float maxPitch = 2f;
    public GameObject SlashVFX;
    public GameObject GroundSplitterVFX;
    public LayerMask Hitable;
    public bool isStillInContact = false;
    public BoxCollider hitBox;

    [Header("Audio")]
    public AudioClip SlashSFX;
    private AudioSource SlashSFXSource;


    public enum WeaponAbilities
    {
        ForwardSlice,
        GroundSplitter
    }
    
    public WeaponAbilities ability;


    private void Start()
    {
        //velocityEstimator = GetComponent<VelocityEstimator>();
        SlashSFXSource = GetComponent<AudioSource>();
    }

    private void Update()
    {   
        if(ability == WeaponAbilities.ForwardSlice)
        {
            ActivateSlashEffect();
        }

        if(ability == WeaponAbilities.GroundSplitter)
        {
            ActivateGroundSplitter();
        }
        
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

    
    private void ActivateGroundSplitter()
    {
        Debug.Log("GroundSplitter");

        if(isStillInContact &&contactPoint.HasValue && contactNormal.HasValue)
        {
            Vector3 point = contactPoint.Value;
            Vector3 normal = contactNormal.Value;
            Quaternion rotation = Quaternion.LookRotation(normal);

            // Offset for above the ground
            Vector3 offset = normal * 0.05f;

            Instantiate(GroundSplitterVFX, point + offset, Quaternion.identity);

            // Clear contact points
            contactPoint = null;
            contactNormal = null;   
        }

    }

    private void OnCollisionEnter(Collision collision)
    {   
        if(((1 << collision.gameObject.layer) & Hitable) != 0)
        {   
            
            
            // Gets the first contact point 
            ContactPoint contact = collision.contacts[0];

            if(contact.thisCollider == hitBox)
            {
                contactPoint = contact.point;
                contactNormal = contact.normal;
                isStillInContact = true;
                Debug.Log(contactPoint);
            }
            
        }
        
    }

    private void OnCollisionExit(Collision collision)
    {
        if(((1 << collision.gameObject.layer) & Hitable) != 0)
        {   

            isStillInContact = false;

            contactPoint = null;
            contactNormal = null;

            Debug.Log("Left contact.");
        }
    }



}
