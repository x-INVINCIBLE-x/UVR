using System;
using DG.Tweening;
using Unity.XR.CoreUtils;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Space]
    [Header("Spike Setup")]
    [Space]
    [SerializeField] GameObject spikes;
    [SerializeField] BoxCollider spikeCollider;
    [SerializeField]private Vector3 movePosition;
    [SerializeField] private float moveDuration;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activateSpikeTrap;
    private Vector3 startPosition;
 
    [Space]
    [Header("Ease Type")]
    [Space]
    [SerializeField] Ease ease; // ease type 


    private void Awake()
    {
        startPosition = spikes.transform.localPosition;
    }
    private void Start()
    {
        spikes.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        ActivateSpike();
        Invoke(nameof(DeactivateSpike),5f);
    }

    private void ActivateSpike()
    {
        spikes.SetActive(true);
        if(activateSpikeTrap != null)
        {
            audioSource.PlayOneShot(activateSpikeTrap);
        }
       
        spikes.transform.DOLocalMove(startPosition + movePosition , moveDuration).SetEase(ease).SetLoops(2,LoopType.Yoyo);
    }

    private void DeactivateSpike()
    {
        spikes.SetActive(false);
    }

}
