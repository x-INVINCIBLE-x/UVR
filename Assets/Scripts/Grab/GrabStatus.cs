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

    public event Action <GrabType> GrabStatusChanged;

    private Transform leftInteractableObject = null;
    private Transform rightInteractableObject = null;


    private void Start()
    {
        LeftInteractor.selectEntered.AddListener(OnLeftHandSelect);
        LeftInteractor.selectExited.AddListener(OnLeftHandDeselect);
        RightInteractor.selectEntered.AddListener(OnRightHandSelect);
        RightInteractor.selectExited.AddListener(OnRightHandDeselect);
    }

    private void OnLeftHandSelect(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out XRGrabInteractable interactable))
        {
            interactable.transform.parent = args.interactorObject.transform;
            leftInteractableObject = interactable.transform;
            ChangeLeftHandStatus(GrabType.Object);
        }
        if (args.interactableObject.transform.TryGetComponent(out ClimbInteractable _))
            ChangeLeftHandStatus(GrabType.Climb);
    }

    private void OnLeftHandDeselect(SelectExitEventArgs args)
    {
        if (leftInteractableObject != null) 
        {
            try
            {
                leftInteractableObject.parent = null;
            }
            catch (Exception)
            {
                Debug.LogError($"Error while releasing object:");
            }
            leftInteractableObject = null;
        }

        ResetHandStatus(Hand.Left);
    }

    private void OnRightHandSelect(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out XRGrabInteractable interactable))
        {
            //interactable.transform.parent = args.interactorObject.transform;
            //rightInteractableObject = interactable.transform;
            ChangeRightHandStatus(GrabType.Object);

        }
        if (args.interactableObject.transform.TryGetComponent(out ClimbInteractable _))
        {
            ChangeRightHandStatus(GrabType.Climb);
        }
    }

    private void OnRightHandDeselect(SelectExitEventArgs args)
    {
        if (rightInteractableObject != null) 
        {
            rightInteractableObject.parent = null;
            rightInteractableObject = null;
        }
        
        ResetHandStatus(Hand.Right);
    }

    public void ChangeHandStatus(Hand hand, GrabType type)
    {
        if (hand == Hand.Left)
            ChangeLeftHandStatus(type);
        else
            ChangeRightHandStatus(type);
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
        GrabStatusChanged?.Invoke(GrabType.Empty);
    }

    public void ChangeLeftHandStatus(GrabType type)
    {
        LeftHand = type;
        GrabStatusChanged?.Invoke(type);
    }

    public void ChangeRightHandStatus(GrabType type)
    {
        RightHand = type;
        GrabStatusChanged?.Invoke(type);
    }

    public bool AreHandsEmpty() => LeftHand == GrabType.Empty && RightHand == GrabType.Empty;
    public bool IsClimbing() => LeftHand == GrabType.Climb || RightHand == GrabType.Climb;
    public bool IsSwinging() => LeftHand == GrabType.Swing || RightHand == GrabType.Swing;

    private void OnDestroy()
    {
        LeftInteractor.selectEntered.RemoveListener(OnLeftHandSelect);
        LeftInteractor.selectExited.RemoveListener(OnLeftHandDeselect);
        RightInteractor.selectEntered.RemoveListener(OnRightHandSelect);
        RightInteractor.selectExited.RemoveListener(OnRightHandDeselect);
    }
}