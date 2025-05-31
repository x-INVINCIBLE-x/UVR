using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ItemSocketSpawner : MonoBehaviour
{   
    // References
    public ItemInfo itemInfo;
    public Transform AttackPoint;

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

        if (itemInfo != null && itemInfo.ItemPrefab != null)
        {
            GameObject itemInstance = Instantiate(itemInfo.ItemPrefab , AttackPoint.position , Quaternion.identity);
            socketInteractor.StartManualInteraction(itemInstance.GetComponent<IXRSelectInteractable>()); // Manually attaches the weapon when it spawns in the socket at the start of the game
            ItemDesc.text = itemInfo.ItemName;
            ItemCost.text = itemInfo.ItemCost.ToString();

            itemXRGrabInteractor = itemInstance.GetComponent<XRGrabInteractable>();
            //itemXRGrabInteractor.enabled = true;

            itemCollider = itemInstance.GetComponent<BoxCollider>();
            //itemCollider.enabled = false;



        }

        
    }


}
