using System;
using UnityEngine;

public class GroundAttacks : WeaponAbilitiesBase
{
    [Header("References for ground attack")]
    [Space]
    private Vector3? contactPoint = null;
    private Vector3? contactNormal = null;
    public GameObject GroundSplitterVFX;
    public LayerMask Hitable;
    public bool isStillInContact = false;
    public BoxCollider hitBox;

    public TypesOfGroundAttacks groundAttackType;
    public enum TypesOfGroundAttacks
    {
        GroundSplitter,
        Homing,
        AOE
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

    }

    protected override void Update()
    {   
        base.Update();
        AllAttacks();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & Hitable) != 0)
        {


            // Gets the first contact point 
            ContactPoint contact = collision.contacts[0];

            if (contact.thisCollider == hitBox)
            {
                contactPoint = contact.point;
                contactNormal = contact.normal;
                isStillInContact = true;
                Debug.Log(contactPoint);
            }

        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & Hitable) != 0)
        {

            isStillInContact = false;

            contactPoint = null;
            contactNormal = null;

            Debug.Log("Left contact.");
        }
    }

    private void GroundSplitter() // Need to add object pool (Important !!!!!)
    {
        Debug.Log("GroundSplitter");
        

        if (isStillInContact && contactPoint.HasValue && contactNormal.HasValue)
        {
            if (!CanAttack()) return;
            ApplyHeat();
            SlashAudio();

            Vector3 point = contactPoint.Value;
            Vector3 normal = contactNormal.Value;
            Quaternion rotation = Quaternion.LookRotation(normal);

            // Offset for above the ground
            Vector3 offset = normal * 0.05f;

            Instantiate(GroundSplitterVFX, point + offset, Quaternion.identity);

            // Clear contact points
            contactPoint = null;
            contactNormal = null;
        }

    }

    protected override void AllAttacks()
    {
        base.AllAttacks();
        if (AbilityEnable == true)
        {
            switch (groundAttackType)
            {
                case TypesOfGroundAttacks.GroundSplitter:
                    GroundSplitter();
                    break;

                case TypesOfGroundAttacks.Homing:
                    HomingGroundAttack();
                    break;

                case TypesOfGroundAttacks.AOE:
                    AOEGroundAttack();
                    break;
            }
        }
        
    }

    private void AOEGroundAttack()
    {
        // Implement AOE attack Here    
    }

    private void HomingGroundAttack()
    {
        // Implement HomingGroundAttack  
    }
}
