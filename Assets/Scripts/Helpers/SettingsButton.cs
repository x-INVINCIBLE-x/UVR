using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI masterVolume;
    [SerializeField] private TextMeshProUGUI bgVolume;
    [SerializeField] private TextMeshProUGUI sfxVolume;

    private void Start()
    {
        masterVolume.text = (AudioManager.Instance.soundMixerManager.masterLevel * 100f).ToString("F0") + "%";
        bgVolume.text = (AudioManager.Instance.soundMixerManager.bgLevel * 100f).ToString("F0") + "%";
        sfxVolume.text = (AudioManager.Instance.soundMixerManager.sfxLevel * 100f).ToString("F0") + "%";
    }
    public void MasterValueUp()
    {
        AudioManager.Instance.soundMixerManager.AddMasterVolume();
        masterVolume.text = (AudioManager.Instance.soundMixerManager.masterLevel * 100f).ToString("F0") + "%";
    }

    public void MasterValueDown()
    {
        AudioManager.Instance.soundMixerManager.SubtractMasterVolume();
        masterVolume.text = (AudioManager.Instance.soundMixerManager.masterLevel * 100f).ToString("F0") + "%";
    }

    public void MusicValueUp()
    {
        AudioManager.Instance.soundMixerManager.AddBGVolume();
        bgVolume.text = (AudioManager.Instance.soundMixerManager.bgLevel * 100f).ToString("F0") + "%";
    }

    public void MusicValueDown()
    {
        AudioManager.Instance.soundMixerManager.SubtractBGVolume();
        bgVolume.text = (AudioManager.Instance.soundMixerManager.bgLevel * 100f).ToString("F0") + "%";
    }

    public void SFXValueUp()
    {
        AudioManager.Instance.soundMixerManager.AddSFXVolume();
        sfxVolume.text = (AudioManager.Instance.soundMixerManager.sfxLevel * 100f).ToString("F0") + "%";
    }

    public void SFXValueDown()
    {
        AudioManager.Instance.soundMixerManager.SubtractSFXVolume();
        sfxVolume.text = (AudioManager.Instance.soundMixerManager.sfxLevel * 100f).ToString("F0") + "%";
    }

    public void EndGame()
    {
        Application.Quit();
    }    
}
