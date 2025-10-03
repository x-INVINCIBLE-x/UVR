using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public void MasterValueUp()
    {
        AudioManager.Instance.soundMixerManager.AddMasterVolume();
    }

    public void MasterValueDown()
    {
        AudioManager.Instance.soundMixerManager.SubtractMasterVolume();
    }

    public void MusicValueUp()
    {
        AudioManager.Instance.soundMixerManager.AddBGVolume();
    }

    public void MusicValueDown()
    {
        AudioManager.Instance.soundMixerManager.SubtractBGVolume();
    }

    public void SFXValueUp()
    {
        AudioManager.Instance.soundMixerManager.AddSFXVolume();
    }

    public void SFXValueDown()
    {
        AudioManager.Instance.soundMixerManager.SubtractSFXVolume();
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
