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

    [Header("Visual Settings")]
    public MeshRenderer[] meshes;                 // Mesh renderers in the 3D model
    private Material[] defaultMaterials;          // Default materials of meshes
    public Material AffordableMaterial;           // Material for affordable state
    public Material UnaffordableMaterial;         // Material for unaffordable state

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
        grabInteractable.hoverEntered.AddListener(OnHoverAttempt);
        grabInteractable.hoverExited.AddListener(OnHoverExit);

        // Player layer for restriction
        playerLayerMask = LayerMask.GetMask("Player");

        // Store default materials
        meshes = GetComponentsInChildren<MeshRenderer>();
        defaultMaterials = new Material[meshes.Length];
        for (int i = 0; i < meshes.Length; i++)
        {
            defaultMaterials[i] = meshes[i].sharedMaterial;
        }
    }

    private void OnHoverAttempt(HoverEnterEventArgs args)
    {
        if (!isShopItem || hasPurchased) return;

        if (AffordableMaterial != null && UnaffordableMaterial != null)
        {
            if (CurrencyManager.Instance != null && CurrencyManager.Instance.Gold >= itemData.ItemCost)
            {
                // Change to affordable material
                foreach (MeshRenderer renderer in meshes)
                {
                    renderer.sharedMaterial = AffordableMaterial;
                }
            }
            else
            {
                // Change to unaffordable material
                foreach (MeshRenderer renderer in meshes)
                {
                    renderer.sharedMaterial = UnaffordableMaterial;
                }
            }
        }
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (!isShopItem || hasPurchased) return;

        CancelInvoke(nameof(ResetMaterial));
        Invoke(nameof(ResetMaterial), 0.5f);
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

            // Reset visuals to default
            ResetMaterial();

            //Remove this script from this object only (not prefab)
            Destroy(this);
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

    private void ResetMaterial()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].sharedMaterial = defaultMaterials[i];
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe
        grabInteractable.selectEntered.RemoveListener(OnGrabAttempt);
        grabInteractable.hoverEntered.RemoveListener(OnHoverAttempt);
        grabInteractable.hoverExited.RemoveListener(OnHoverExit);
    }
}
