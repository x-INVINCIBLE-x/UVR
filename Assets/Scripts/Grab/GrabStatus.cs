using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;

public enum GrabType
{
    Empty,
    Climb,
    Swing,
    Object
}

public enum Hand
{
    Left,
    Right
}

public class GrabStatus : MonoBehaviour
{
    [field: SerializeField] public NearFarInteractor LeftInteractor { get; private set; }
    [field: SerializeField] public NearFarInteractor RightInteractor { get; private set; }

    [field: SerializeField] public GrabType LeftHand {  get; private set; }
    [field: SerializeField] public GrabType RightHand {  get; private set; }

    private void Start()
    {
        LeftInteractor.selectEntered.AddListener(OnLeftHandSelect);
        LeftInteractor.selectExited.AddListener(OnLeftHandDeselect);
        RightInteractor.selectEntered.AddListener(OnRightHandSelect);
        RightInteractor.selectExited.AddListener(OnRightHandDeselect);
    }

    private void OnLeftHandSelect(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out XRGrabInteractable statusUpdater))
            ChangeLeftHandStatus(GrabType.Object);
        if (args.interactorObject.transform.TryGetComponent(out ClimbInteractable _))
            ChangeLeftHandStatus(GrabType.Climb);
    }

    private void OnLeftHandDeselect(SelectExitEventArgs args) => ResetHandStatus(Hand.Left);
    private void OnRightHandSelect(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out XRGrabInteractable statusUpdater))
            ChangeRightHandStatus(GrabType.Object);
        if (args.interactorObject.transform.parent.TryGetComponent(out ClimbInteractable _))
            ChangeRightHandStatus(GrabType.Climb);
    }

    private void OnRightHandDeselect(SelectExitEventArgs args) => ResetHandStatus(Hand.Right);

    public void ChangeHandStatus(Hand hand, GrabType type)
    {
        if (hand == Hand.Left)
            LeftHand = type;
        else
            RightHand = type;
    }

    public GrabType GetStatus(Hand hand)
    {
        if (hand == Hand.Left) 
            return LeftHand;

        return RightHand;
    }
    
    public void ResetHandStatus(Hand hand)
    {
        if (hand == Hand.Left) LeftHand = GrabType.Empty;
        else RightHand = GrabType.Empty;
    }

    public void ChangeLeftHandStatus(GrabType type) => LeftHand = type;
    public void ChangeRightHandStatus(GrabType type) => RightHand = type;
    public bool AreHandsEmpty() => LeftHand == GrabType.Empty && RightHand == GrabType.Empty;
    public bool IsClimbing() => LeftHand == GrabType.Climb || RightHand == GrabType.Climb;

    private void OnDestroy()
    {
        LeftInteractor.selectEntered.RemoveListener(OnLeftHandSelect);
        LeftInteractor.selectExited.RemoveListener(OnLeftHandDeselect);
        RightInteractor.selectEntered.RemoveListener(OnRightHandSelect);
        RightInteractor.selectExited.RemoveListener(OnRightHandDeselect);
    }
}