using UnityEngine;

public class BroomStick : MonoBehaviour
{
   
    // Goal is to create a flying brrom stick for experiencing flight in vr


    private Rigidbody rb_BroomStick;
    [SerializeField] private BoxCollider broomHandle;
 
    [SerializeField] private float broomSpeed;
   

    private void Awake()
    {
        rb_BroomStick = GetComponent<Rigidbody>();
        broomHandle = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        //BroomInputReader();
        BroomMovementHandler();
    }


    private void BroomInputReader()
    {
        float xDirection = gameObject.transform.localPosition.x;
        float yDirection = gameObject.transform.localPosition.y;
        float zDirection = gameObject.transform.localPosition.z;


        // Printing values of broom on console
        //Debug.Log(xDirection);
        //Debug.Log(yDirection);
        //Debug.Log(zDirection);
    }

    private void BroomMovementHandler()
    {
        Vector3 broomDirection = gameObject.transform.localPosition;
        
        Debug.Log(broomDirection);
    }

    




}
