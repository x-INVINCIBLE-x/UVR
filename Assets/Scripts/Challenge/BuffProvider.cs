using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(BoxCollider))]
public class BuffProvider : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Buff buffToApply;
    private DungeonBuffHandler handler;
    private bool canGrab = true;

    private XRSocketInteractor socket;
    private TemporaryBuffs temporaryBuff;
    private BuffStatsUI buffView;

    private int layerMask;

    private void Start()
    {
        layerMask = LayerMask.GetMask("Player");
    }

    public void Initialize(DungeonBuffHandler _handler, Buff _buff, XRSocketInteractor _socket, TemporaryBuffs _temporaryBuff)
    {
        socket = _socket;
        socket.StartManualInteraction(GetComponent<IXRSelectInteractable>());

        handler = _handler;
        buffToApply = _buff;
        temporaryBuff = _temporaryBuff;

        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.hoverEntered.AddListener(OnHoverAttempt);
        grabInteractable.hoverExited.AddListener(OnHoverExit);
        grabInteractable.selectEntered.AddListener(OnGrabAttempt);
        grabInteractable.selectExited.AddListener(OnGrabDrop);
        grabInteractable.activated.AddListener(OnActivationAttempt);

        canGrab = true;
    }

    private void OnHoverAttempt(HoverEnterEventArgs arg0)
    {
        Debug.Log("Hover Enter");

        if (!canGrab)
            return;

        handler.DisableCardInteraction(this);
    }

    private void OnHoverExit(HoverExitEventArgs arg0)
    {
        Debug.Log("Hover Exit");

        if (!canGrab)
            return;

        handler.EnableCardInteraction();
    }

    private void OnGrabAttempt(SelectEnterEventArgs args)
    {
        if ((layerMask & (1 << args.interactorObject.transform.gameObject.layer)) == 0)
        {
            return;
        }

        if (!canGrab)
        {
            CancelGrab(args);
            return;
        }

        socket.enabled = false;

        Debug.Log("Grab Enter");

        handler.DisableCardInteraction(this);
    }

    private void OnGrabDrop(SelectExitEventArgs arg0)
    {
        if ((layerMask & (1 << arg0.interactorObject.transform.gameObject.layer)) == 0)
        {
            return;
        }

        socket.enabled = true;

        Debug.Log("Grab Exit");
        StartCoroutine(ReattachToSocketNextFrame());
    }

    private IEnumerator ReattachToSocketNextFrame()
    {
        yield return new WaitForSeconds(0.1f); 

        if (socket && grabInteractable)
        {
            transform.position = socket.attachTransform.position;
            transform.rotation = socket.attachTransform.rotation;

            socket.StartManualInteraction(GetComponent<IXRSelectInteractable>());
        }
    }


    private void OnActivationAttempt(ActivateEventArgs arg0)
    {
        if (temporaryBuff != null)
        {
            Debug.Log("Buff Activated");
            temporaryBuff.AddBuff(buffToApply);
            handler.BuffPicked();
        }
    }

    private void CancelGrab(SelectEnterEventArgs args)
    {
        if (grabInteractable.isSelected && grabInteractable.interactionManager != null)
        {
            grabInteractable.interactionManager.SelectExit(args.interactorObject, grabInteractable);
        }
    }

    public void SetGrab(bool status) => canGrab = status;

    public void Close()
    {
        grabInteractable.hoverEntered.RemoveListener(OnHoverAttempt);
        grabInteractable.hoverExited.RemoveListener(OnHoverExit);
        grabInteractable.selectEntered.RemoveListener(OnGrabAttempt);
        grabInteractable.selectExited.RemoveListener(OnGrabDrop);
        grabInteractable.activated.RemoveListener(OnActivationAttempt);

        StartCoroutine(CloseRoutine());
    }

    private IEnumerator CloseRoutine()
    {
        //GetComponent<Renderer>().material = dissolveMaterial;
        //GetComponent<BoxCollider>().enabled = false;

        yield return new WaitForSeconds(handler.Dissolve_Duration);
        Destroy(gameObject);
    }
}
