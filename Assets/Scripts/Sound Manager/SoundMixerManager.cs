using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float volumeStep = 0.05f;

    [SerializeField, Range(minVolume, maxVolume)] private float masterLevel = 1f;
    [SerializeField, Range(minVolume, maxVolume)] private float bgLevel = 1f;
    [SerializeField, Range(minVolume, maxVolume)] private float sfxLevel = 1f;

    private const float maxVolume = 1f;
    private const float minVolume = 0.0001f;

    private void Start()
    {
        // Initialize mixer with inspector values
        masterLevel = ApplyVolume("MasterVolume", masterLevel);
        bgLevel = ApplyVolume("BGVolume", bgLevel);
        sfxLevel = ApplyVolume("SFXVolume", sfxLevel);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SubtractMasterVolume();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddMasterVolume();
        }
    }
    private float ApplyVolume(string parameter, float level)
    {
        float clamped = Mathf.Clamp(level, minVolume, maxVolume);
        audioMixer.SetFloat(parameter, Mathf.Log10(clamped) * 20f);
        return clamped;
    }

    // Volume Setters
    public void SetMasterVolume(float level) => masterLevel = ApplyVolume("MasterVolume", level);
    public void SetBGVolume(float level) => bgLevel = ApplyVolume("BGVolume", level);
    public void SetSFXVolume(float level) => sfxLevel = ApplyVolume("SFXVolume", level);

    // Button Methods
    public void AddMasterVolume() => SetMasterVolume(masterLevel + volumeStep);
    public void SubtractMasterVolume() => SetMasterVolume(masterLevel - volumeStep);

    public void AddBGVolume() => SetBGVolume(bgLevel + volumeStep);
    public void SubtractBGVolume() => SetBGVolume(bgLevel - volumeStep);

    public void AddSFXVolume() => SetSFXVolume(sfxLevel + volumeStep);
    public void SubtractSFXVolume() => SetSFXVolume(sfxLevel - volumeStep);
}
