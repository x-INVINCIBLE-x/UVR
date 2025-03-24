using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSocketItemInput : UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor
{
    public TargetTag targetTag;
    public Item currentItem;

    private InventoryManager inventoryManager;

    public float itemAcceptDelay = 1f;
    private Coroutine currentCoroutine = null;

    protected override void Start()
    {
        base.Start();

        inventoryManager = InventoryManager.Instance;
    }

    public override bool CanHover(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && interactable.transform.CompareTag(targetTag.ToString());
    }

    public override bool CanSelect(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && interactable.transform.CompareTag(targetTag.ToString());
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (!args.interactableObject.transform.TryGetComponent(out Item currItem))
        {
            return;
        }

        currentItem = currItem;
        currentCoroutine = StartCoroutine(AddToInventory());
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentItem = null;
    }

    private IEnumerator AddToInventory()
    {
        yield return new WaitForSeconds(itemAcceptDelay);

        if (currentItem == null)
        {
            yield break;
        }

        if (inventoryManager.Additem(currentItem.data))
        {
            Destroy(currentItem.gameObject);
            currentItem = null;
        }
    }
}