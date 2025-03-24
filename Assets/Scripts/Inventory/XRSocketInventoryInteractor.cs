using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSocketInventoryInteractor : UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor
{
    public TargetTag targetTag;
    public Item currentWeapon = null;
    public Transform meleeAttatchTransform;
    public Transform rangedAttachTransform;

    public UIToogleHandler closeHandler;
    private InventoryManager inventoryManager;
    private bool isSafe = false;
    private bool hasWeaponInSlot = false;

    protected override void Awake()
    {
        base.Awake();
        inventoryManager = InventoryManager.Instance;
        closeHandler = GetComponentInParent<UIToogleHandler>();
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

        if (args.interactableObject.transform.TryGetComponent(out Item weapon))
        {
            currentWeapon = weapon;
        }

        if (hasWeaponInSlot)
        {
            return;
        }

        hasWeaponInSlot = true;
        InventoryItem item = new InventoryItem(weapon.data);
        InventoryManager.Instance.AddItemFromSocket(item, this);
        //inventoryManager.AddItemFromSocket(weapon, this);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (!isSafe)
        {
            return;
        }

        InventoryManager.Instance.RemoveItemFromSocket(this);
        currentWeapon = null;
        hasWeaponInSlot = false;
        //inventoryManager.RemoveItemFromSocket(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        closeHandler.OnClose += HandleClose;

        isSafe = true;
        InventoryItem item = InventoryManager.Instance.GetItem(this);
        if (item != null)
        {
            currentWeapon = Instantiate(item.data.Model, transform.position + new Vector3(0, 0.5f, 0), attachTransform.rotation).GetComponent<Item>();
        }
    }

    private void HandleClose()
    {
        isSafe = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        closeHandler.OnClose -= HandleClose;

        if (currentWeapon == null)
        {
            return;
        }

        Destroy(currentWeapon.gameObject);
        currentWeapon = null;
    }
}
