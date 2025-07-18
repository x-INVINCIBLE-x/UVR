using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PurchasableItem : MonoBehaviour
{
    [Header("References")]
    [Space]
    public ItemData itemData;
    public bool isShopItem = false;

    private XRGrabInteractable grabInteractable;
    private bool hasPurchased = false;

    [Header("Visuals Settings")]

    [Space]

    public MeshRenderer[] meshes; // Mesh renderer component of all the meshes in 3d model 
    public Material[] defaultMaterial; // default material of all mesh renderers

    [Space]

    public Material AffordableMaterial; // Material for items that we can buy
    public Material UnaffordableMaterial; // Material for item that we cannot buy

    [Space]

    private int layerMask; 

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabAttempt);
        grabInteractable.hoverEntered.AddListener(OnHoverAttempt);
        grabInteractable.hoverExited.AddListener(OnHoverExit);
        layerMask = LayerMask.GetMask("Player");

        // Highlighting 
        meshes = GetComponentsInChildren<MeshRenderer>();
        defaultMaterial = new Material[meshes.Length];

        for (int i = 0; i < meshes.Length; i++)
        {
            defaultMaterial[i] = meshes[i].material;
            //Debug.Log("Mat" + meshes[i].material.name);
        }
    }


    private void OnHoverAttempt(HoverEnterEventArgs arg0)
    {   
        if(AffordableMaterial != null && UnaffordableMaterial != null && hasPurchased == false )
        {
            if (CurrencyManager.Instance.Gold >= itemData.ItemCost)
            {
                // Change to affordable material
                foreach (MeshRenderer renderer in meshes)
                {
                    renderer.material = AffordableMaterial;
                }

            }
            else if (CurrencyManager.Instance.Gold < itemData.ItemCost)
            {
                // Change to Unaffordable material
                foreach (MeshRenderer renderer in meshes)
                {
                    renderer.material = UnaffordableMaterial;
                }

            }
        }
    
    }
    private void OnHoverExit(HoverExitEventArgs arg0)
    {
        Invoke(nameof(ResetMaterial), 0.5f);// Reset to default material  
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabAttempt);
        grabInteractable.hoverEntered.RemoveListener(OnHoverAttempt);
        grabInteractable.hoverExited.RemoveListener(OnHoverExit);

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

        var moneyManager = CurrencyManager.Instance;

        if (moneyManager == null)
        {   
            Debug.Log("Money Manager is Missing");
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

    private void ResetMaterial()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].material = defaultMaterial[i];
        }
    }
}