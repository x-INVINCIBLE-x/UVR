using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FlameThrower : SprayingWeapons
{
    [Header("FlameThrower Settings")]
    //[SerializeField]private bool isHolding = false;
    [SerializeField] private GameObject sprayVFX;
    [SerializeField] private CapsuleCollider attackRadius;

    protected override void Awake()
    {
        base.Awake();
        attackRadius = GetComponentInChildren<CapsuleCollider>(); 
        //sprayVFX = transform.Find("Flamethrower")?.gameObject;

        if (sprayVFX != null)
            sprayVFX.SetActive(false);

        if (attackRadius != null)
            attackRadius.enabled = false;
    }

    protected override void ActivateWeapon(ActivateEventArgs args)
    {
        base.ActivateWeapon(args);
        //isHolding = true;
        StartSpraying();       
    }

    protected override void DeactivateWeapon(DeactivateEventArgs args)
    {
        base.DeactivateWeapon(args);
        //isHolding = false;
        StopSpraying();
    }

    protected override void StartSpraying()
    {
        base.StartSpraying();

        if(sprayVFX != null)
        {
            sprayVFX.SetActive(true);
        }

        if(attackRadius != null)
        {
            attackRadius.enabled = true;
        }
    }

    protected override void StopSpraying()
    {
        base.StopSpraying();

        if(sprayVFX != null)
        {
            sprayVFX.gameObject.SetActive(false);
        }
        
        if(attackRadius != null)
        {
            attackRadius.enabled = false;
        }
        
    }
}