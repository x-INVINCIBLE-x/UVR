using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class InteractorTouchTracker : MonoBehaviour
{
    [SerializeField] private XRBaseInputInteractor interactor;

    public IXRInteractable CurrentHover { get; private set; }
    public IXRInteractable CurrentSelect { get; private set; }

    private void Awake()
    {
        // Subscribe to hover + select events
        interactor.hoverEntered.AddListener(OnHoverEnter);
        interactor.hoverExited.AddListener(OnHoverExit);
        interactor.selectEntered.AddListener(OnSelectEnter);
        interactor.selectExited.AddListener(OnSelectExit);
    }

    private void OnDestroy()
    {
        // Unsubscribe
        interactor.hoverEntered.RemoveListener(OnHoverEnter);
        interactor.hoverExited.RemoveListener(OnHoverExit);
        interactor.selectEntered.RemoveListener(OnSelectEnter);
        interactor.selectExited.RemoveListener(OnSelectExit);
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        CurrentHover = args.interactableObject;
        Debug.Log($"{interactor.name} is hovering {CurrentHover.transform.name}");
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        Debug.Log($"{interactor.name} stopped hovering {args.interactableObject.transform.name}");
        if (CurrentHover == args.interactableObject)
            CurrentHover = null;
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        CurrentSelect = args.interactableObject;
        Debug.Log($"{interactor.name} selected {CurrentSelect.transform.name}");
    }

    private void OnSelectExit(SelectExitEventArgs args)
    {
        Debug.Log($"{interactor.name} released {args.interactableObject.transform.name}");
        if (CurrentSelect == args.interactableObject)
            CurrentSelect = null;
    }
}
