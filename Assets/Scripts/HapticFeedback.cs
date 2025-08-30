using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[System.Serializable]
public class HapticSettings
{ //sdasd
    [Range(0f, 1f)]
    public float intensity;
    public float duration;

    public void TriggerHaptics(BaseInteractionEventArgs eventArgs)
    {
        if(eventArgs.interactorObject is XRBaseInputInteractor controllerInteractor && controllerInteractor != null)
        {
            controllerInteractor.SendHapticImpulse(intensity,duration);
        }       
    }
}

public class HapticFeedback : MonoBehaviour
{
    public HapticSettings onSelectEntered;
    public HapticSettings onSelectExited;
    public HapticSettings onActivate;
    public HapticSettings onDeactivate;

    private XRBaseInteractable interactable; 
    private void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
        if (interactable == null)
        {
            Debug.LogError("No XRBaseInteractable found on " + gameObject.name);
            return;
        }

        SetupGrabEvents();
    }

    private  void SetupGrabEvents()
    {
        interactable.selectEntered.AddListener(onSelectEntered.TriggerHaptics);
        interactable.selectExited.AddListener(onSelectExited.TriggerHaptics);
        interactable.activated.AddListener(onActivate.TriggerHaptics);
        interactable.deactivated.AddListener(onDeactivate.TriggerHaptics);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(onSelectEntered.TriggerHaptics);
        interactable.selectExited.RemoveListener(onSelectExited.TriggerHaptics);
        interactable.activated.RemoveListener(onActivate.TriggerHaptics);
        interactable.deactivated.RemoveListener(onDeactivate.TriggerHaptics);
    }

}
