using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PurchasableItem : MonoBehaviour
{
    public ItemData itemData;
    public bool isShopItem = false;

    private XRGrabInteractable grabInteractable;
    private bool hasPurchased = false;

    int layerMask; 
    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabAttempt);
        layerMask = LayerMask.GetMask("Player");
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabAttempt);
    }

    private void OnGrabAttempt(SelectEnterEventArgs args)
    {
        if ((layerMask & (1 << args.interactorObject.transform.gameObject.layer)) == 0)
        {
            Debug.Log(args.interactorObject.transform.gameObject.name);
            Debug.Log("Block");
            return;
        }

        if (!isShopItem)
            return;

        if (hasPurchased)
            return;

        var moneyManager = MoneyManager.Instance;

        if (moneyManager == null)
        {
            Debug.Log("Money Manager is Missing");
            CancelGrab(args);
            return;
        }

        if (moneyManager.Gold >= itemData.ItemCost)
        {
            moneyManager.SpendMoney(itemData.ItemCost);
            hasPurchased = true;
            Debug.Log($"{itemData.Name} purchased for {itemData.ItemCost}");
        }

        else
        {
            Debug.Log($" not enough money to buy {itemData.Name}");
            CancelGrab(args);
        }
    }

    private void CancelGrab(SelectEnterEventArgs args)
    {
        if (grabInteractable.isSelected && grabInteractable.interactionManager != null)
        {
            grabInteractable.interactionManager.SelectExit(args.interactorObject, grabInteractable);
        }
    }
}