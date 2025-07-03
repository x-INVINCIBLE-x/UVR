using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PurchasableItem : XRSocketInteractor
{
    [Space]
    [Header("References")]
    public ItemData itemData;
    public bool isShopItem = false;
    [Space]

    [Space]
    [Header("Condition")]
    [Space]
    private XRGrabInteractable grabInteractable;
    private bool hasPurchased = false;
    private bool canAffordItem = false;

    [Space]
    [Header("Visual Effect")]
    [Space]
    private MeshRenderer[] Meshes;
    private Material[] originalMaterials;
    public Material affordableMaterial;
    public Material unaffordableMaterial;

    protected override void Awake()
    {
        base.Awake();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Get all the mesh renderers for changing the material 
        Meshes = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[Meshes.Length];
        for (int i = 0; i < Meshes.Length; i++)
        {
            originalMaterials[i] = Meshes[i].material;
            //Debug.Log("Mat" + Meshes[i].material.name);
        }
    }


    public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractor(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isShopItem && !hasPurchased)
        {   
            // Checking if Player has enough money to buy the item 
            bool currentAffordability = MoneyManager.Instance != null && MoneyManager.Instance.Gold >= itemData.ItemCost;

            if (currentAffordability != canAffordItem)
            {
                canAffordItem = currentAffordability; // Update check for weapon
            }
        }
    }


    public override bool CanHover(IXRHoverInteractable interactable)
    {
        if (isShopItem && !hasPurchased && !canAffordItem)
        {
            Debug.Log($" not enough money to buy {itemData.Name}");
            return false;
        }

        return base.CanHover(interactable);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {   
        if(isShopItem && !hasPurchased)
        {
            if(canAffordItem && MoneyManager.Instance.SpendMoney(itemData.ItemCost))
            {
                hasPurchased = true;
                return base.CanSelect(interactable);
            }
            return false;
        }
        return base.CanSelect(interactable);
    }

    private void HoverVisuals()
    {
        // Implement code for changing the material of all the meshes when we hover in (can buy or cannot buy) and out (reset to original)
    }

}
