using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{   
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindFirstObjectByType<AudioManager>();
                if(instance == null)
                {
                    instance = new GameObject("Spawned AudioManager" , typeof(AudioManager)).GetComponent<AudioManager>();
                }
            }

            return instance;
        }

        private set
        {
            instance = value;            
        }
    }

    private AudioSource musicSource;
    private AudioSource musicSource2;
    private AudioSource sfxSource;

    private bool isPlayingMusicSource1; // if it is true then musicSource is playing , If it is false then musicSource 2 is playing

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        musicSource = this.gameObject.AddComponent<AudioSource>();
        musicSource2 = this.gameObject.AddComponent<AudioSource>();
        sfxSource = this.gameObject.AddComponent<AudioSource>();

        // Loop the music sources
        musicSource.loop = true;
        musicSource2.loop = true;
    }

    public void PlayMusic(AudioClip musicClip)
    {   
        // Determine Active music source

        AudioSource activeSource = (isPlayingMusicSource1) ? musicSource : musicSource2;

        activeSource.clip = musicClip;
        activeSource.volume = 1;
        activeSource.Play();
    }

    public void PlayMusicWithFade(AudioClip newClip,float transitionTime = 1.0f)
    {   
        // Determine which AudioSource is active
        AudioSource activeSource = (isPlayingMusicSource1) ? musicSource : musicSource2;
        StartCoroutine(UpdateMusicWithFade(activeSource, newClip, transitionTime));
    }

    public void PlayMusicWithCrossFade(AudioClip newClip, float transitionTime = 1.0f)
    {
        // Determine which AudioSource is active
        AudioSource activeSource = (isPlayingMusicSource1) ? musicSource : musicSource2; // Terinary operation to check if musicSource is playing (Read Above)
        AudioSource newSource = (isPlayingMusicSource1) ? musicSource2 : musicSource; // Swap the terinary, condition to play musicSource2 if the musicSource is playing

        // Swap Source
        isPlayingMusicSource1 = !isPlayingMusicSource1;

        // Set the fields of the audioSource , then start the courutine for cross fade
        newSource.clip = newClip;
        newSource.Play();
        StartCoroutine(UpdateMusicWithCrossFade(activeSource,newSource,transitionTime));
    }

    private IEnumerator UpdateMusicWithCrossFade(AudioSource original, AudioSource newSource, float transitionTime)
    {
        float t = 0.0f;
        
        for (t = 0.0f; t <= transitionTime; t += Time.deltaTime)
        {
            original.volume = (1 - (t / transitionTime));
            newSource.volume = (t / transitionTime);
            yield return null;
        }

        original.Stop();
    }

    private IEnumerator UpdateMusicWithFade(AudioSource activeSource , AudioClip newClip , float transitionTime)
    {
        if (!activeSource.isPlaying)
        {
            activeSource.Play();
        }

        float t = 0.0f;

        // FADE OUT
        for ( t = 0.0f; t < transitionTime ; t+= Time.deltaTime)
        {
            activeSource.volume = (1 - (t / transitionTime));
            yield return null;
        }

        activeSource.Stop();
        activeSource.clip = newClip;
        activeSource.Play();

        // FADE IN
        for (t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (t / transitionTime);
            yield return null;
        }
    }
    public void PlaySFX(AudioClip sfxClip)
    {
        sfxSource.PlayOneShot(sfxClip);
    }

    public void PlaySFX(AudioClip sfxClip, float volume)
    {
        sfxSource.PlayOneShot(sfxClip,volume);
    }

    // Extra Function for setting Volume

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        musicSource2.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

}
