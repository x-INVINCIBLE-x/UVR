using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ItemsHandler : MonoBehaviour
{
    private InventoryManager inventoryManager;
    public XRSocketDisplayInteractor outputInteractor;

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
    }

    public void DisplayItem(ItemData data)
    {
        outputInteractor.Setup(data);
    }

    public void RemoveItem(ItemData data)
    {
        inventoryManager.RemoveItem(data);
    }
}
