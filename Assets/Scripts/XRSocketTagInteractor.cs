using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public enum TargetTag
{
    EnergyKey,
    Weapon,
    InventoryItem
}

public class XRSocketTagInteractor : UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor
{
    public TargetTag targetTag;

    public override bool CanHover(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && interactable.transform.CompareTag(targetTag.ToString());
    }

    public override bool CanSelect(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && interactable.transform.CompareTag(targetTag.ToString());
    }

}
