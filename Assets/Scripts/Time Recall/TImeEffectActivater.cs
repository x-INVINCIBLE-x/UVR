using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


public class TImeEffectActivater : MonoBehaviour
{
    public Volume recallVolume;

    public AudioSource TimeAudioSource;
    public AudioClip timeStopStart;
    public AudioClip timeStopEnd;

    private RecallSettings recallEffect;

    [SerializeField] private float wipeSizeSpeed = 5f;
    
    
    public Camera mainCamera;

    private Coroutine wipeCoroutine = null; 
    public float lastTimeActivated = -10f;


    private void Start()
    {
        if(recallVolume != null)
        {
            recallVolume.profile.TryGet(out recallEffect);
        }



        recallEffect.active = false;
        //recallEffect.wipeSize.overrideState = false;
        //recallEffect.wipeOriginPoint.overrideState = false;
        //mainCamera = Camera.main;

      
        recallEffect.wipeSize.overrideState = true;
        recallEffect.wipeOriginPoint.overrideState = true;
        
        

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && wipeCoroutine == null)
            wipeCoroutine =  StartCoroutine(UpdateWipeValue(true));
        else if (Input.GetKeyDown(KeyCode.D) && wipeCoroutine == null)
            wipeCoroutine = StartCoroutine(UpdateWipeValue(false));
    }

    IEnumerator UpdateWipeValue(bool status)
    {   
        if(status == true) { recallEffect.active = true; }
        while (status == true && recallEffect.wipeSize.value < 2.5f)
        {
            yield return new WaitForEndOfFrame();
            recallEffect.wipeSize.value += wipeSizeSpeed * Time.unscaledDeltaTime;
            recallEffect.noiseScale.value = Mathf.Lerp(100f, 199f, 0.2f);
            //TimeAudioSource.PlayOneShot(timeStopStart);
        }
        
        while (status == false && recallEffect.wipeSize.value > 0)
        {
            yield return new WaitForEndOfFrame();
            recallEffect.wipeSize.value -= wipeSizeSpeed * Time.unscaledDeltaTime;
            recallEffect.noiseScale.value = Mathf.Lerp(199f,100f, 0.2f);
            //TimeAudioSource.PlayOneShot(timeStopEnd);
        }

        if (status == false) { recallEffect.active = false; }
        wipeCoroutine = null;
    }
}
