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

    private void Start()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();

        if (itemData != null && itemData.Model != null)
        {
            GameObject itemInstance = Instantiate(itemData.Model , AttachPoint.position , Quaternion.identity);
            socketInteractor.StartManualInteraction(itemInstance.GetComponent<IXRSelectInteractable>()); // Manually attaches the weapon when it spawns in the socket at the start of the game
            ItemDesc.text = itemData.Name;
            ItemCost.text = itemData.ItemCost.ToString();

            PurchasableItem purchasable = itemInstance.GetComponent<PurchasableItem>();
            if(purchasable != null)
            {
                purchasable.itemData = itemData;
                purchasable.isShopItem = true; // this bool makes it a shop item when instantiate (only the instance of the object)

            }



        }

        
    }


}
