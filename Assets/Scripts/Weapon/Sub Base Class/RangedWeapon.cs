using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class RangedWeapon : Weapon
{   
    [SerializeField] protected float shootingForce;
    [SerializeField] protected float recoilForce;
    //[SerializeField] protected float damage;
    [SerializeField] protected GameObject muzzleVFX;
    [SerializeField] protected AudioClip shootSFX;
    protected AudioSource WeaponAudioSource;

    

    protected override void Awake()
    {   
        base.Awake();

        WeaponAudioSource = GetComponent<AudioSource>();
    }
    // Pistol , rifle and projectile shooting related weapons
    protected virtual void Shoot()
    {
        ApplyRecoil();
    }

    private void ApplyRecoil()
    {
        rigidBody.AddRelativeForce(Vector3.back * recoilForce, ForceMode.Impulse);
    }

    public float GetShootingForce()
    {
        return shootingForce;
    }
}
