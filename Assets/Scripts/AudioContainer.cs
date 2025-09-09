using UnityEngine;

public class AudioContainer : MonoBehaviour
{
    [SerializeField] private AudioSource mainAudioSource;
    [SerializeField] private AudioClip MainMenuUI;
    [SerializeField] private AudioClip SubMenuUI;
    [SerializeField] private AudioClip Touch;


    void Start()
    {
        AudioManager.Instance.PlaySFX3d(mainAudioSource,MainMenuUI);
    }

    //public void playSound(AudioClip audioClip)
    //{   
    //    mainAudioSource.loop = false;
    //    mainAudioSource.spatialBlend = 1;
    //    mainAudioSource.PlayOneShot(audioClip);
    //}





}
