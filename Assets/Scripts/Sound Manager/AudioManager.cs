using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
    private Coroutine musicCoroutine;
    private Coroutine sfxCoroutine;

    private bool isPlayingMusicSource1; // if it is true then musicSource is playing , If it is false then musicSource 2 is playing

    [SerializeField] private int maxSimultaneousSFX = 3;
    [SerializeField] private AudioSource audioSourcePrefab;
    [Space]
    [Header("Mixer Settings")]
    [SerializeField] private AudioMixerGroup MusicMixer;
    [SerializeField] private AudioMixerGroup SFXMixer;

    private Queue<AudioSource> audioPool = new Queue<AudioSource>();
    private List<AudioSource> activeSources = new List<AudioSource>();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        // Intializing Audio Sources onto Audio Manager
        musicSource = this.gameObject.AddComponent<AudioSource>();
        musicSource2 = this.gameObject.AddComponent<AudioSource>();
        sfxSource = this.gameObject.AddComponent<AudioSource>();

        // Intializing  Output(AudioMixer)
        musicSource.outputAudioMixerGroup = MusicMixer;
        musicSource2.outputAudioMixerGroup = MusicMixer;
        sfxSource.outputAudioMixerGroup = SFXMixer;

        // Loop the music sources
        musicSource.loop = true;
        musicSource2.loop = true;
        // Intialization
        isPlayingMusicSource1 = true;

        if (audioSourcePrefab == null)
        {
            Debug.LogWarning("AudioManager: AudioSource Prefab is not assigned!");
            return;
        }
        for (int i = 0; i < maxSimultaneousSFX; i++)
        {
            AudioSource src = Instantiate(audioSourcePrefab, transform);
            src.playOnAwake = false;
            src.spatialBlend = 0f; // force 2D
            src.gameObject.SetActive(false);

            audioPool.Enqueue(src);
        }
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
        // Coroutine Check
        if (musicCoroutine!= null)
        {
            StopCoroutine(musicCoroutine);
        }
        musicCoroutine = StartCoroutine(UpdateMusicWithFade(activeSource, newClip, transitionTime));
    }

    // Play SFX sounds in 3d with Loop
    public void PlaySFXLoop3d(AudioSource sfxSource, AudioClip sfxClip, bool play)
    {
        if (sfxClip == null) return;

        sfxSource.loop = true;
        sfxSource.spatialBlend = 1; //3d
        sfxSource.clip = sfxClip;
        // Check if to play audioclip
        if (play)
        {
            sfxSource.Play();
        }
        else
        {
            sfxSource.Stop();
        }
    }

    // Play Sfx Sounds in 3d
    public void PlaySFX3d(AudioSource sfxSource, AudioClip sfxClip , float volume = 1)
    {
        if (sfxClip == null) return;

        // Stop any loop before playing one-shot
        sfxSource.loop = false;
        sfxSource.spatialBlend = 1; // 3d sound 
        //sfxSource.Stop();

        sfxSource.PlayOneShot(sfxClip);
    }

    // Play SFX sounds in 2d
    public void PlaySFX2d(AudioSource sfxSource, AudioClip sfxClip , float volume)
    {
        if (sfxClip == null) return;

        // Stop any loop before playing one-shot
        sfxSource.spatialBlend = 0; // 2d Sound
        sfxSource.loop = false;
        sfxSource.Stop();

        sfxSource.PlayOneShot(sfxClip, volume);
    }

    // Play SFX with Charging effect
    public void PlaySFXChargingSound3d(AudioSource sfxSource, AudioClip sfxClip, float ChargeTime)
    {
        if (sfxClip == null) return;

        sfxSource.loop = true;
        sfxSource.spatialBlend = 1;
        if (sfxCoroutine != null)
        {
            StopCoroutine(sfxCoroutine);
        }

        sfxCoroutine = StartCoroutine(ChargingSound(sfxSource, sfxClip, ChargeTime)); // Charging coroutine set

    }

    /// <summary>
    /// This function plays a one-shot sound effect using an audio pool to manage simultaneous sounds.
    /// System SFX is not bound to any entity and is played in 2D space.
    /// There is a limit to the number of simultaneous SFX that can be played, defined by maxSimultaneousSFX.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    public void PlaySystemSFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        // Check if pool has free source
        if (audioPool.Count > 0)
        {
            AudioSource src = audioPool.Dequeue();
            activeSources.Add(src);

            src.gameObject.SetActive(true);
            src.clip = clip;
            src.volume = volume;
            src.Play();

            StartCoroutine(ReturnToPool(src, clip.length));
        }
        else
        {
            Debug.Log("SFXHandler: Max simultaneous SFX reached!");
        }
    }

    private IEnumerator ReturnToPool(AudioSource src, float delay)
    {
        yield return new WaitForSeconds(delay);

        src.Stop();
        src.clip = null;
        src.gameObject.SetActive(false);

        activeSources.Remove(src);
        audioPool.Enqueue(src);
    }
    public void PlayMusicWithCrossFade(AudioClip newClip, float transitionTime = 1.0f)
    {
        if (newClip == null) return;

        // Determine which AudioSource is active
        AudioSource activeSource = (isPlayingMusicSource1) ? musicSource : musicSource2; // Terinary operation to check if musicSource is playing (Read Above)
        AudioSource newSource = (isPlayingMusicSource1) ? musicSource2 : musicSource; // Swap the terinary, condition to play musicSource2 if the musicSource is playing

        // Swap Source
        isPlayingMusicSource1 = !isPlayingMusicSource1;

        // Set the fields of the audioSource , then start the courutine for cross fade
        newSource.clip = newClip;
        newSource.volume = 0; // Start at 0 volume
        newSource.Play();

        // Coroutine Check
        if (musicCoroutine != null)
        {
            StopCoroutine(musicCoroutine);
        }
        musicCoroutine = StartCoroutine(UpdateMusicWithCrossFade(activeSource,newSource,transitionTime));
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
        musicCoroutine = null;
    }

    private IEnumerator UpdateMusicWithFade(AudioSource activeSource, AudioClip newClip, float transitionTime)
    {
        if (!activeSource.isPlaying)
        {
            activeSource.Play();
        }

        float t = 0.0f;

        // FADE OUT
        for (t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (1 - (t / transitionTime));
            yield return null;
        }

        activeSource.Stop();
        activeSource.clip = newClip;
        activeSource.volume = 0; // Explicitly set to 0 before playing (just a brute force way to ensure volume is 0 when new clip is played)
        activeSource.Play();

        // FADE IN
        for (t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (t / transitionTime);
            yield return null;
        }

        musicCoroutine = null;
    }

    private IEnumerator ChargingSound(AudioSource activeSource, AudioClip sfxClip, float ChargeTime)
    {
        float timer = 0.0f;
        activeSource.volume = 0; // Setting of volume to zero when starting charging
        activeSource.clip = sfxClip;
        activeSource.Play();

        for (timer = 0 ; timer <= ChargeTime; timer += Time.deltaTime)
        {
            activeSource.volume = (timer / ChargeTime);
            yield return null;
        }
        activeSource.Stop(); // End sound after completion
        sfxCoroutine = null;
    }


    public void StopAllSound(AudioSource audioSource)
    {   // For cases where coroutine doesn't stop
        
        //audioSource.Stop();
        StopAllCoroutines();
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
