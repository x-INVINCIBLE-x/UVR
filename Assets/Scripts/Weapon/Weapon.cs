using System.Collections.Generic;
using UnityEngine;
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
        abilityHandler = AbilityHandler.Instance;
        SetupInteractableWeaponEvents();

        abilities = new Dictionary<AbilityType, System.Action>
        {
            {AbilityType.None, () => HandleNormal() },
            {AbilityType.Explode, () => HandleExplosive() },
            {AbilityType.Freeze, () => HandleFreeze() }
        };
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
        //Debug.Log(abilityHandler.ActiveAbility);
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
