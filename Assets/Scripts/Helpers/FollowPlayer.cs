using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform playerTransform;
    [SerializeField] private bool followY = false;
    public Vector3 offset;
    private Vector3 followPosition;

    private void Start()
    {
        playerTransform = PlayerManager.instance.player;

        followPosition = playerTransform.position + offset;
        Follow();
    }

    private void Update()
    {
        followPosition = playerTransform.position + offset;

        if (!followY)
        {
            followPosition.y = transform.position.y;
        }

        Follow();
    }

    private void Follow()
    {
        transform.position = followPosition;
    }
}
