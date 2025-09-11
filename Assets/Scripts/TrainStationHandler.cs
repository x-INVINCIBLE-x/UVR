using UnityEngine;

public class TrainStationHandler : MonoBehaviour
{
    // Attach this to Train Lever 

    [Space]
    [Header("Train Animation and Sound")]
    [SerializeField] private Animation trainEngineOn;
    //[SerializeField] private Animation trainStartRunnning;
    [SerializeField] private AudioSource trainAudioSource;
    [SerializeField] private AudioClip trainWhitsle;
    [SerializeField] private AudioClip trainEngine;
    [SerializeField] private AudioClip TrainStartSFX;

    [Space]
    [Header("Train VFX Setup")]
    [SerializeField] private GameObject[] steamVFX;

    [Space]
    [Header("References")]
    [SerializeField] private Collider trainOnDirection;

    private void TrainTravelActivator()
    {
        // Manages the scenes and the vfx settings 
        TrainAnimationHandler();
        TrainAudioHandler();
    }
    
    public void TrainDestinationSetter()
    {
        // Sets the destination to travel to through button events
        // Write the logic for scene transition
    }

    private void OnTriggerEnter(Collider other)
    {   
        // Donot forget to set the tag for the train lever (IMP)
        // Also set up trigger collider for the lever to work it should collide with the trainOnDirection collider
        if(other.CompareTag("Train Lever"))
        {
            TrainTravelActivator();
        }
    }

    private void TrainAnimationHandler()
    {
        if (trainEngineOn != null)
        {
            trainEngineOn.Play();
            foreach (GameObject steam in steamVFX)
            {
                steam.SetActive(true);
            }
        }
    }

    private void TrainAudioHandler()
    {
        if (trainWhitsle != null)
        {
            trainAudioSource.spatialBlend = 1;
            trainAudioSource.PlayOneShot(trainWhitsle);

            if (trainAudioSource.isPlaying == false)
            {
                trainAudioSource.clip = trainEngine;
                trainAudioSource.loop = true;// Safe check will remove later
                trainAudioSource.Play();
            }
        }
    }

    private void OnDisable()
    {
        foreach (GameObject steam in steamVFX)
        {
            steam.SetActive(false);
        }
    }
}
