using GLTFast.Schema;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

using static UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.HapticsUtility;

public class BroomStickController : MonoBehaviour
{
    [Header("Components")]

    private Rigidbody rb_BroomStick;
    [SerializeField] private BoxCollider broomHandle;
    [SerializeField] public GameObject player;

    [Header("Broom Settings")]

    private float currentBroomSpeed;
    [SerializeField] private float broomRotationSpeed = 5f;
    //[SerializeField] private float minBroomSpeed = 5f;
    [SerializeField] private float endBroomSpeed = 15f;
    [SerializeField] private float accelerationRate = 3f;
    
    

    private void Awake()
    {
        rb_BroomStick = GetComponent<Rigidbody>();
        broomHandle = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        RotationHandler();
        VelocityHandler();
        

    }

    private void RotationHandler()
    {
        Quaternion targetRotation = player.transform.rotation;
        rb_BroomStick.MoveRotation(Quaternion.Slerp(rb_BroomStick.rotation, targetRotation, Time.deltaTime * broomRotationSpeed));
    }

    private void VelocityHandler()
    {
        if (Input.GetKey(KeyCode.Space)) // Space bar is heldown to apply speed
        {
            ApplyForce();
            MovePlayerWithBroom();
            

        }
        else
        {
            BrakeForce();
            MovePlayerWithBroom();
        }
    }
    private void MovePlayerWithBroom()
    {
        if (player != null)
        {
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
        }
    }

    private void ApplyForce()
    {   
        // Applies Force to the broomstick for moving(flight)

        currentBroomSpeed = Mathf.Lerp(currentBroomSpeed, endBroomSpeed , accelerationRate * Time.deltaTime); // Smooth increase of speed with time from current speed to max speed for the broom stick
        rb_BroomStick.AddForce(transform.up * currentBroomSpeed, ForceMode.Acceleration);// Physics based movement
    }

    private void BrakeForce()
    {
        currentBroomSpeed = Mathf.Lerp(currentBroomSpeed, 0, accelerationRate * Time.deltaTime);
        rb_BroomStick.linearVelocity = transform.up * currentBroomSpeed;
    }
}
