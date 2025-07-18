using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform playerTransform;
    [SerializeField] private bool followY = false;
    [SerializeField] private bool followPlayerOnce = false;
    public Vector3 offset;
    private Vector3 followPosition;

    private void Start()
    {
        if (PlayerManager.instance != null)
            playerTransform = PlayerManager.instance.Player.playerBody; // --- Possible hazard ---- //
        else
        {
            Debug.Log("PlayerManager instance not found by " + gameObject.name);
            return;
        }

        followPosition = playerTransform.position + offset;
        Follow();
    }

    private void Update()
    {
        if (followPlayerOnce)
        {
            return;
        }

        if (playerTransform == null)
        {
            enabled = false;
            return;
        }

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
