using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Beamgun : SprayingWeapons
{
    [Header("Beam Gun Settings")]
    //[SerializeField]private bool isHolding = false;
    [SerializeField] private GameObject beamVFX; // TODO: Convert this Into DamageOnTouch
    [SerializeField] private CapsuleCollider attackRadius;
    [SerializeField] private AudioClip beamSFX;

    protected override void Awake()
    {
        base.Awake();
        attackRadius = GetComponentInChildren<CapsuleCollider>();
        //sprayVFX = transform.Find("Flamethrower")?.gameObject;

        if (beamVFX != null)
            beamVFX.SetActive(false);

        if (attackRadius != null)
            attackRadius.enabled = false;

        WeaponAudioSource.loop = true;
        WeaponAudioSource.clip = beamSFX;
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

        WeaponAudioSource.Play();

        if (beamVFX != null)
        {
            beamVFX.SetActive(true);
        }

        if (attackRadius != null)
        {
            attackRadius.enabled = true;
        }
    }

    protected override void StopSpraying()
    {
        base.StopSpraying();

        WeaponAudioSource.Stop();

        if (beamVFX != null)
        {
            beamVFX.gameObject.SetActive(false);
        }

        if (attackRadius != null)
        {
            attackRadius.enabled = false;
        }

    }
}
