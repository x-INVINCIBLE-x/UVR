using UnityEngine;
using DG.Tweening;

public class Traps : MonoBehaviour
{   
    // Disclaimer : there are things we need to make sure before tweening 
    // A tweening object if it has a rigidbody should be kinematic
    // A tweening method like DOMove should only be called in start
    // The z forward transform of the launcher object should be the where be want to shoot it

    [Header("Trap Type")]
    [Space]
    public TypesofTraps trapTypes;
    public enum TypesofTraps
    {
        staticTrap,
        movingTrap,
        spinbladesTrap,
        AreaeffectTraps,
        LauncherTraps
    }

    //public TypeofEase easeType;
    //public enum TypeofEase
    //{   
    //    Ease_linear,
    //    Ease_in,
    //    Ease_out,
    //    Ease_inout,
    //    Ease_InOutQuad
    //}


    [Space]
    [Header("References")]
    [Space]

    public Collider[] hurtBoxes;
    private Rigidbody trapRb;
    

    [Space]
    [Header("Moving trap Setup")]
    [Space]
    public Vector3 movetoOffset;
    private Vector3 startPosition;
    [SerializeField] private float moveDuration;

    [Space]
    [Header("Ease Type")]
    [Space]
    [SerializeField] Ease ease; // ease type 

    [Space]
    [Header("loopType")]
    [Space]
    [SerializeField] LoopType loopType; // Looping type

    [Space]
    [Header("Launcher trap")]
    [Space]

    public GameObject launcherObject;

    [Space]
    [Header("Area Effect Setup")]
    [Space]

    public GameObject areaEffectVFX;
    private GameObject currentAreaEffect;
    public Vector3 areaEffectVFXOffset;

    private void Awake()
    {
        trapRb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }
    private void Start()
    {
        CheckTrapType();
    }

    private void Update()
    {
        

    }

    public void CheckTrapType()
    {
        switch (trapTypes)
        {
            case TypesofTraps.staticTrap:
                StaticTrapActivate();
                break;

            case TypesofTraps.movingTrap:
                MovingTrapActivate();
                break;

            case TypesofTraps.spinbladesTrap:
                SpinbladesTrapActivate();
                break;

        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.CompareTag("Player"))
        {
            Debug.Log("Trigger Activated");

            switch (trapTypes)
            {
                case TypesofTraps.LauncherTraps:
                    LauncherTrapActivate();
                    break;

                case TypesofTraps.AreaeffectTraps:
                    AreaeffectTrapActivate();
                    break;
            }
        }
    }


    private void OnTriggerExit(Collider trigger)
    {
        if (trigger.gameObject.CompareTag("Player"))
        {
            Debug.Log("Trigger Activated");

            switch (trapTypes)
            {
                case TypesofTraps.AreaeffectTraps:
                    Destroy(currentAreaEffect,1f);
                    break;
            }
        }
    }
    public void StaticTrapActivate()
    {
        //Debug.Log("Static Trap Activated");
    }

    public void MovingTrapActivate()
    {
        //Debug.Log("Moving Trap Activated");
        // Add logic for Moving traps
        Debug.Log($"Moving Trap Activated with ease: {ease}");
        transform.DOMove(startPosition + movetoOffset, moveDuration).SetEase(ease).SetLoops(-1,loopType);
        RotateTrap();

    }

    public void SpinbladesTrapActivate()
    {
        //Debug.Log("Spinblades Trap Activated");
    }

    public void AreaeffectTrapActivate()
    {
        //Debug.Log("Areaeffect Trap Activated");
        if(areaEffectVFX == null) return;

        currentAreaEffect = Instantiate(areaEffectVFX, startPosition + areaEffectVFXOffset, Quaternion.identity);
    }

    public void LauncherTrapActivate()
    {
        if (launcherObject == null) return;
        //Debug.Log("Launcher Trap Activated");

        Rigidbody launcherObjectRb = Instantiate(launcherObject, startPosition + new Vector3(0f, 0f, 0.1f), transform.rotation).GetComponent<Rigidbody>();
        launcherObjectRb.AddForce(transform.forward * 32f, ForceMode.Impulse);
    }

    public void RotateTrap()
    {
        transform.DORotate(new Vector3(0,360f,0), 1.5f ,RotateMode.FastBeyond360).SetLoops(-1,LoopType.Restart).SetRelative().SetEase(Ease.Linear);
    }
   

}
