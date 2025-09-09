
using UnityEngine;

public class AudioTester : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audio3d;
    [SerializeField] private AudioClip audio3dloop;
    [SerializeField] private AudioClip audio2d;
    [SerializeField] private AudioClip audioCharging;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.Instance.PlaySFX3d(audioSource,audio3d);
            Debug.Log("3d");
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {   
            AudioManager.Instance.PlaySFXLoop3d(audioSource,audio3dloop,true);
            Debug.Log("Loop 3d");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {   
            AudioManager.Instance.PlaySFXLoop3d(audioSource,audio3dloop,false);
            Debug.Log("Loop 3d");
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            AudioManager.Instance.PlaySFX2d(audioSource,audio2d,1);
            Debug.Log("2d");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            AudioManager.Instance.PlaySFXChargingSound3d(audioSource,audioCharging,10);
            Debug.Log("Chatging");
        }
     

    }

}
