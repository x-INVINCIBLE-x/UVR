using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TrainStationHandler : MonoBehaviour
{
    // Attach this to Train Lever 

    [Space]
    [Header("Train Animation and Sound")]
    [SerializeField] private Animator trainAnimator;
    //[SerializeField] private Animation trainStartRunnning;
    [SerializeField] private AudioSource trainAudioSource1;
    [SerializeField] private AudioSource trainAudioSource2;
    [SerializeField] private AudioClip trainWhistleSFX;
    [SerializeField] private AudioClip trainEngineSFX;

    [Space]
    [Header("Train VFX Setup")]
    [SerializeField] private GameObject[] Vfxs;

    [Space]
    [Header("Lever Reference")]
    [SerializeField] private GameObject lever;
    private XRGrabInteractable interactableLever;
    private HingeJoint leverjoint;
    private float minJointAngle;
    private float maxJointAngle;
    private float currentAngle;
    private bool isGrabbed;
    private bool hasActivated;

    private void Awake()
    {
        interactableLever = lever.GetComponent<XRGrabInteractable>();
        leverjoint = lever.GetComponent<HingeJoint>();
       
    }

    private void Start()
    {
        if (leverjoint != null)
        {
            minJointAngle = leverjoint.limits.min;
            maxJointAngle = leverjoint.limits.max;
        }

        if (interactableLever != null)
        {
            interactableLever.selectEntered.AddListener(GrabLever);
            interactableLever.selectExited.AddListener(ReleaseLever);
        }
    }

    private void Update()
    {
        if (isGrabbed && !hasActivated) 
        {
            CheckLever();     
        }
    }
    private void GrabLever(SelectEnterEventArgs arg0)
    {
        isGrabbed = true;
        hasActivated = false;
    }

    private void ReleaseLever(SelectExitEventArgs arg0)
    {
        isGrabbed = false;
        //interactableLever.selectEntered.RemoveListener(GrabLever);
        //interactableLever.selectExited.RemoveListener(ReleaseLever);
    }


    private void TrainTravelActivator()
    {
        // Manages the scenes and the vfx settings and everything
        TrainAnimationHandler();
        TrainAudioHandler();
        TrainDestinationSetter();
    }
    
    public void TrainDestinationSetter()
    {
        // Sets the destination to travel to through button events
        // Write the logic for scene transition

    }


    private void CheckLever()
    {
        if (LeverAtAnglePosition())
        {
            hasActivated = true;
            Debug.Log("Train Lever Activated");
            TrainTravelActivator();
        }
    }
    private bool LeverAtAnglePosition()
    {
        return Mathf.Approximately(leverjoint.angle, minJointAngle) ||
               leverjoint.angle <= minJointAngle + 2f; // Angle torelance (if the lever is somehow stuck just above)
    }

    private void TrainAnimationHandler()
    {
        if (trainAnimator != null && Vfxs != null)
        {
            trainAnimator.Play("Train Run");
            foreach (GameObject steam in Vfxs)
            {
                steam.SetActive(true);
            }
        }
    }

    private void TrainAudioHandler()
    {
        if (trainWhistleSFX != null)
        {
            trainAudioSource1.spatialBlend = 1;
            trainAudioSource1.PlayOneShot(trainWhistleSFX);
        }
        if (trainEngineSFX != null)
        {
            trainAudioSource2.spatialBlend = 1;
            trainAudioSource2.loop = true;
            trainAudioSource2.clip = trainEngineSFX;
            trainAudioSource2.Play();
        }
    }

    private void OnDisable()
    {
        interactableLever.selectEntered.RemoveListener(GrabLever);
        interactableLever.selectExited.RemoveListener(ReleaseLever);
        foreach (GameObject steam in Vfxs)
        {
            steam.SetActive(false);
        }
    }
}
