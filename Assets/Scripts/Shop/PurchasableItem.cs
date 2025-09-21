using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PurchasableItem : MonoBehaviour
{
    [Header("References")]
    public ItemData itemData;
    [Tooltip("Mark true if this item is sold in the shop.")]
    public bool isShopItem = false;

    private XRGrabInteractable grabInteractable;
    [SerializeField] private bool hasPurchased = false;

    private int playerLayerMask;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (!isShopItem)
        {
            // Normal item → already considered purchased
            hasPurchased = true;
        }

        // Subscribe to events
        grabInteractable.selectEntered.AddListener(OnGrabAttempt);

        // Player layer for restriction
        playerLayerMask = LayerMask.GetMask("Player");
    }

    private void OnGrabAttempt(SelectEnterEventArgs args)
    {
        // Check if grabber is player
        if ((playerLayerMask & (1 << args.interactorObject.transform.gameObject.layer)) == 0)
        {
            Debug.Log("Grab blocked: not player");
            return;
        }

        // Normal item → always grabbable
        if (!isShopItem) return;

        // Already purchased → grabbable
        if (hasPurchased) return;

        var moneyManager = CurrencyManager.Instance;
        if (moneyManager == null)
        {
            Debug.LogWarning("CurrencyManager is missing!");
            CancelGrab(args);
            return;
        }

        if (moneyManager.Gold >= itemData.ItemCost)
        {
            moneyManager.SpendGold(itemData.ItemCost);
            hasPurchased = true;
            Debug.Log($"{itemData.Name} purchased for {itemData.ItemCost}");
        }
        else
        {
            Debug.Log($"Not enough money to buy {itemData.Name}");
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

    private void OnDestroy()
    {
        // Unsubscribe
        grabInteractable.selectEntered.RemoveListener(OnGrabAttempt);
    }
}
