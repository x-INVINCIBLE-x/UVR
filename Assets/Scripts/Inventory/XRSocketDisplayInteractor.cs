using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSocketDisplayInteractor : UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor
{
    private readonly TargetTag targetTag = TargetTag.InventoryItem;
    private ItemData currDisplayData = null;
    public bool isSafe = false;
    public GameObject currItem = null;
    private UIToogleHandler closeHandler;

    protected override void Awake()
    {
        base.Awake();
        closeHandler = GetComponentInParent<UIToogleHandler>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        closeHandler.OnClose += HandleClose;
        isSafe = true;
    }

    public void Setup(ItemData newDisplayData)
    {
        Debug.Log(currDisplayData + " -> " +  newDisplayData);
        if (currDisplayData == newDisplayData)
        {
            Debug.Log("Returned");
            return;
        }

        Clear();

        isSafe = true;
        gameObject.SetActive(true);
        currItem = Instantiate(newDisplayData.Model, transform.position, Quaternion.identity);
        currDisplayData = newDisplayData;
    }

    public override bool CanHover(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && interactable.transform.CompareTag(targetTag.ToString());
    }

    public override bool CanSelect(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && interactable.transform.CompareTag(targetTag.ToString());
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (!isSafe)
        {
            if (currItem != null)
            {
                Destroy(currItem);
            }
            return;
        }

        currItem = null;
        InventoryManager.Instance.RemoveItem(currDisplayData);
        HandleClose();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        closeHandler.OnClose -= HandleClose;
    }

    public void Clear()
    {
        isSafe = false;
        if (currItem != null)
        {
            Destroy(currItem);
        }
        isSafe = true;
    }

    private void HandleClose()
    {
        isSafe = false;

        if (currItem != null)
        {
            Destroy(currItem);
        }

        currItem = null;
        currDisplayData = null;

        gameObject.SetActive(false);
    }

}
