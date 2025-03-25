using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform playerTransform;
    public Vector3 offset;

    private void Start()
    {
        playerTransform = PlayerManager.instance.player;
    }

    private void Update()
    {
        transform.position = playerTransform.position + offset;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(playerTransform.position + offset, 0.1f);
    }
}
