using UnityEngine;

public class GroundSplitter : WeaponAbilitiesBase
{
    private Vector3? contactPoint = null;
    private Vector3? contactNormal = null;

    public GameObject GroundSplitterVFX;

    public LayerMask Hitable;
    public bool isStillInContact = false;
    public BoxCollider hitBox;


    protected override void Start()
    {
        base.Start();

    }

    private void Update()
    {
        ActivateGroundSplitter();
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

    private void ActivateGroundSplitter()
    {
        Debug.Log("GroundSplitter");

        if (isStillInContact && contactPoint.HasValue && contactNormal.HasValue)
        {
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
}
