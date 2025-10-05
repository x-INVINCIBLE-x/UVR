using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ItemSocketSpawner : MonoBehaviour
{   
    // References
    public ItemData itemData;
    public Transform AttachPoint;
    

    // Interactors(XR)
    private XRSocketInteractor socketInteractor;
    private XRGrabInteractable itemXRGrabInteractor;
    private BoxCollider itemCollider;

    // UI
    public TextMeshProUGUI ItemDesc;
    public TextMeshProUGUI ItemCost;

    private void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
    }

    private void Start()
    {
        StartCoroutine(SpawnItemAfterFrame());
    }

    private IEnumerator SpawnItemAfterFrame()
    {
        yield return null;

        if (itemData != null && itemData.Model != null)
        {
            GameObject itemInstance = Instantiate(itemData.Model, AttachPoint.position, AttachPoint.rotation);

            IXRSelectInteractable interactable = itemInstance.GetComponent<IXRSelectInteractable>();
            if (interactable != null)
                socketInteractor.StartManualInteraction(interactable);

            ItemDesc.text = itemData.Name;
            ItemCost.text = itemData.ItemCost.ToString();

            PurchasableItem purchasable = itemInstance.GetComponent<PurchasableItem>();
            if (purchasable != null)
            {
                purchasable.itemData = itemData;
                purchasable.isShopItem = true;
            }
        }
    }

}
