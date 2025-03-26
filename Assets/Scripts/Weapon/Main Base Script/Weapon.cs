using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Weapon : MonoBehaviour
{
    private XRGrabInteractable interactableWeapon;
    protected Rigidbody rigidBody;
    protected AbilityHandler abilityHandler;

    public Dictionary<AbilityType, System.Action> abilities;

    protected virtual void Awake()
    {
        interactableWeapon = GetComponent<XRGrabInteractable>();
        rigidBody = GetComponent<Rigidbody>();
        SetupInteractableWeaponEvents();

        abilities = new Dictionary<AbilityType, System.Action>
        {
            {AbilityType.None, () => HandleNormal() },
            {AbilityType.Explode, () => HandleExplosive() },
            {AbilityType.Freeze, () => HandleFreeze() }
        };
    }

    private void Start()
    {
        abilityHandler = AbilityHandler.Instance;
    }

    private void SetupInteractableWeaponEvents()
    {
        interactableWeapon.selectEntered.AddListener(PickUpWeapon);
        interactableWeapon.selectExited.AddListener(DropWeapon);
        interactableWeapon.activated.AddListener(ActivateWeapon);
        interactableWeapon.deactivated.AddListener(DeactivateWeapon);
    }

    private void PickUpWeapon(SelectEnterEventArgs args)
    {
        args.interactorObject.transform.GetComponent<MeshHidder>()?.Hide();
    }

    private void DropWeapon(SelectExitEventArgs args)
    {
        args.interactorObject.transform.GetComponent<MeshHidder>()?.Show();
    }
    
    protected virtual void ActivateWeapon(ActivateEventArgs args)
    {
    }

    protected virtual void DeactivateWeapon(DeactivateEventArgs args)
    {
    }

    protected virtual void HandleNormal()
    {

    }

    protected virtual void HandleExplosive()
    {

    }

    protected virtual void HandleFreeze() 
    { 

    }
}
