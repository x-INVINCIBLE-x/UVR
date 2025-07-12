using System.Collections;
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

    [Header("CoolDown")]
    [SerializeField] protected float maxHeatLimit = 100f; // this is max fill up value , if it is full then the weapon will go into cooldown phase
    [SerializeField] protected float coolDownRate = 5f; // Rate at which cooldown takes place (during cooldown we cannot shoot)
    [SerializeField] protected float overheatResistance = 6f; // the value which is used to deduce the currentheat if it has not reached the max limit value (continous)
    [SerializeField] protected float currentHeat = 0f; // current heat is how much the current weapon is heated 
    [SerializeField] protected float heatBuildup = 0f; // Heat buildup per shot
    [SerializeField] protected bool isOverheated = false;

    protected Coroutine cooldownCoroutine;


    protected override void Awake()
    {   
        base.Awake();

        WeaponAudioSource = GetComponent<AudioSource>();
    }
    // Pistol , rifle and projectile shooting related weapons
    protected virtual void Shoot()
    {   
        if(!CanShoot()) return;

        ApplyHeat();
        ApplyRecoil();
    }
    protected bool CanShoot()
    {
        if (isOverheated)
        {
            return false;
        }

        return true;
    }

    private void ApplyRecoil()
    {
        rigidBody.AddRelativeForce(Vector3.back * recoilForce, ForceMode.Impulse);
    }

    public float GetShootingForce()
    {
        return shootingForce;
    }

    protected virtual IEnumerator ReduceValueOverTime()
    {
        while(currentHeat > 0 && !isOverheated)
        {
            currentHeat -= overheatResistance * Time.deltaTime;
            currentHeat = Mathf.Clamp(currentHeat,0, maxHeatLimit);
            yield return null;
            
        }
    }

    protected virtual IEnumerator CoolDownPhase()
    {
        while(currentHeat > 0)
        {
            currentHeat -= coolDownRate * Time.deltaTime;
            currentHeat = Mathf.Clamp(currentHeat, 0, maxHeatLimit);
            yield return null;
        }
        
        isOverheated = false;
        cooldownCoroutine = null; // Stop coroutine plus its reference 

        if (currentHeat > 0)
        {
            cooldownCoroutine = StartCoroutine(ReduceValueOverTime());
        }

    } 

    protected virtual void ApplyHeat()
    {   
        currentHeat += heatBuildup;

        if(currentHeat >= maxHeatLimit)
        {
            Overheat();
        }

        else if (!isOverheated)
        {
            //Debug.Log("ReduceValueOverTime");
            cooldownCoroutine = StartCoroutine(ReduceValueOverTime());
        }
    }
    protected virtual void Overheat()
    {
        isOverheated = true;

        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        cooldownCoroutine = StartCoroutine(CoolDownPhase());
    }

    private void OnDisable()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }
    }

}
