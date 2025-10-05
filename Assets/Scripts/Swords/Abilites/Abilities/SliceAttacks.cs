using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class SliceAttacks : WeaponAbilitiesBase
{
    [Header("Refernces and Settings")]
    [Space]
    public PhysicsProjectile SlashVFX;
    [SerializeField] private float velocityThreshold = 3f;
    [SerializeField] private Transform slashSpawn;
    [SerializeField] private float force;

    [Header("Boomerang Settings")]
    public Vector3 movetoOffset;
    private Vector3 startPosition;
    [SerializeField] private float moveDuration;
    [SerializeField] private float rotationSpeed;

    [Header("Slash Data")]
    public TypesOfSlices sliceAttackType;
    [SerializeField] private float lifeTime;
    [SerializeField] private AttackData attackData;

    [Header("Activation Delay")]
    [SerializeField] private float activationDelay = 0.5f;
    private bool isDelayedActivation = false;


    public enum TypesOfSlices
    {
        Straight,
        Boomerang,
        Spining,
        AOE
    }

    protected void Update()
    {   
        AllAttacks();                
    }


    protected override void AllAttacks()
    {
        base.AllAttacks();

        if (AbilityEnable == true && !isDelayedActivation)
        {
            isDelayedActivation = true;
            Invoke(nameof(DelayedActivation), activationDelay);
        }
        else if (AbilityEnable == false)
        {
            CancelInvoke(nameof(DelayedActivation));
            WeaponVFX.SetActive(false);
            isDelayedActivation = false;
        }
    }

    private void DelayedActivation()
    {
        if (AbilityEnable)
        {
            if (WeaponVFX != null)
            {
                WeaponVFX.SetActive(true);
            }
            
            switch (sliceAttackType)
            {
                case TypesOfSlices.Straight:
                    StraightSliceAttack();
                    break;

                case TypesOfSlices.Boomerang:
                    BoomerangSliceAttack();
                    break;

                case TypesOfSlices.Spining:
                    SpiningSliceAttack();
                    break;

                case TypesOfSlices.AOE:
                    AoeSliceAttack();
                    break;

            }
        }
        else
        {
            WeaponVFX.SetActive(false);
        }
        isDelayedActivation = false;
    }
   
    private void ActivateSlashEffect()
    {
        float velocity = velocityEstimator.GetVelocityEstimate().magnitude;

        if (velocity > velocityThreshold)
        {
            Debug.Log(velocity);
            //Instantiate(SlashVFX, gameObject.transform.position,Quaternion.identity);
            //Destroy(SlashVFX,1f);*
            GameObject newSlashVFX = ObjectPool.instance.GetObject(SlashVFX.gameObject, slashSpawn);
            newSlashVFX.GetComponent<PhysicsProjectile>().Init(lifeTime, attackData);
            Rigidbody slashBody = newSlashVFX.GetComponent<Rigidbody>();

            slashBody.linearVelocity = slashSpawn.transform.forward * force;

            SlashAudio();
            ObjectPool.instance.ReturnObject(SlashVFX.gameObject,2f);

        }

        
    }

    // Sends the slice projectile attack forward in a straight line
    private void StraightSliceAttack()
    {
        if (AbilityEnable == false) return;
        if(!VelocityChecker()) return; // checks the velocity of the weapon that is swung
        if (!CanAttack())
        {   
            WeaponVFX.SetActive (false);
            return;
        }
       
        ApplyHeat();
        SlashAudio();

        GameObject newSlashVFX = ObjectPool.instance.GetObject(SlashVFX.gameObject, slashSpawn);
        newSlashVFX.GetComponent<PhysicsProjectile>().Init(lifeTime, attackData);
        newSlashVFX.transform.localRotation = gameObject.transform.rotation;
        Rigidbody slashBody = newSlashVFX.GetComponent<Rigidbody>();
        slashBody.linearVelocity = transform.forward * force;
        ObjectPool.instance.ReturnObject(newSlashVFX, 2f);   
    }

    // Sends the projectile in as a spinning attack that keeps spining for some time 
    private void SpiningSliceAttack()
    {
        if (AbilityEnable == false) return;
        if (!VelocityChecker()) return; // checks the velocity of the weapon that is swung
        if (!CanAttack())
        {
            WeaponVFX.SetActive(false);
            return;
        }
        ApplyHeat();
        SlashAudio();

        GameObject newSlashVFX = ObjectPool.instance.GetObject(SlashVFX.gameObject, slashSpawn);
        newSlashVFX.GetComponent<PhysicsProjectile>().Init(lifeTime, attackData);
        newSlashVFX.transform.localRotation = gameObject.transform.rotation;
        Rigidbody slashBody = newSlashVFX.GetComponent<Rigidbody>();

        // Postion Estimations
        startPosition = slashSpawn.position;
        newSlashVFX.transform.position = startPosition;
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 targetPosition = startPosition + cameraForward * movetoOffset.magnitude;

        // Dotween Implementation
        slashBody.transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutSine);
        slashBody.DORotate(new Vector3(0, 360, 0), rotationSpeed , RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetRelative().SetEase(Ease.Linear);

        //Object Pool
        ObjectPool.instance.ReturnObject(newSlashVFX, moveDuration * 2f);
    }

    // Sends a attack that returns back to the player 
    private void BoomerangSliceAttack()
    {
        if (AbilityEnable == false) return;
        if (!VelocityChecker()) return; // checks the velocity of the weapon that is swung
        if (!CanAttack())
        {
            WeaponVFX.SetActive(false);
            return;
        }
        ApplyHeat();
        SlashAudio();

        WeaponVFX.SetActive(true);
        GameObject newSlashVFX = ObjectPool.instance.GetObject(SlashVFX.gameObject, slashSpawn);
        newSlashVFX.GetComponent<PhysicsProjectile>().Init(lifeTime, attackData);
        Rigidbody slashBody = newSlashVFX.GetComponent<Rigidbody>();
        
        // Postion Estimations
        startPosition = slashSpawn.position;
        newSlashVFX.transform.position = startPosition;
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 targetPosition = startPosition + cameraForward * movetoOffset.magnitude;

        // Dotween Implementation
        slashBody.transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);

        slashBody.DORotate(new Vector3(0, 360, 0), rotationSpeed, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetRelative().SetEase(Ease.Linear);

        //Object Pool
        ObjectPool.instance.ReturnObject(newSlashVFX, moveDuration * 2f);
    }
    private void AoeSliceAttack()
    {
        
    }

    private bool VelocityChecker()
    {
        float velocity = velocityEstimator.GetVelocityEstimate().magnitude;

        if (velocity > velocityThreshold) 
        {
            return true; 
        }
        return false;

    }

   
    

}
