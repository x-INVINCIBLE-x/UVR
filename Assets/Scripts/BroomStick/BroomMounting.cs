using UnityEngine;

public class BroomMounting : MonoBehaviour
{
    [SerializeField] public GameObject broom;
    [SerializeField] public Transform player;
    [SerializeField] public Transform attachPoint;
    [SerializeField] public Rigidbody rbPlayer;
    //[SerializeField] public bool isMounted = false;

    
    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.CompareTag("Player") && player.parent != attachPoint)
        {
            player.SetParent(attachPoint);
            rbPlayer.isKinematic = true;
            player.localPosition = Vector3.zero;
            player.localRotation = Quaternion.identity; // Aligns with attachPoint's rotation
            
        }
    }
    private void OnTriggerExit(Collider trigger)
    {
        if (trigger.CompareTag("Player"))
        {
            player.SetParent(null);
            rbPlayer.isKinematic = false;
            player.position += transform.up * 0.5f; // Move slightly forward to avoid re-triggering
        }
    }



}
