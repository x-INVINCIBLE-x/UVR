using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class UseableItem : MonoBehaviour
{
    private XRGrabInteractable interactableItem;

    private void Awake()
    {
        //interactableItem.selectEntered.AddListener(PickUpItem);
        //interactableItem.selectExited.AddListener(DropItem);
        interactableItem.activated.AddListener(UseItem);
        //interactableItem.deactivated.AddListener(DeactivateWeapon);
    }

    private void PickUpItem(SelectEnterEventArgs args)
    {
        args.interactorObject.transform.GetComponent<MeshHidder>()?.Hide();
    }

    private void DropItem(SelectExitEventArgs args)
    {
        args.interactorObject.transform.GetComponent<MeshHidder>()?.Show();
    }

    protected virtual void UseItem(ActivateEventArgs args)
    {
        Debug.Log("use item");
    }

    protected virtual void DeactivateWeapon(DeactivateEventArgs args)
    {

    }

}
