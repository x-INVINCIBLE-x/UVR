using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class FollowPlayer : MonoBehaviour
{
    private Transform playerTransform;

    [Header("Lazy Follow")]
    [SerializeField] private bool useLazyFollow = false;
    [SerializeField] private bool rotateWithPlayer = false;

    [Header("Settings")]
    [SerializeField] private bool followY = false;
    [SerializeField] private bool followPlayerOnce = false;
    public Vector3 offset;
    private Vector3 followPosition;

    private void Start()
    {
        if (useLazyFollow)
        {
            if (!TryGetComponent<LazyFollow>(out var lazyFollow))
            {
                lazyFollow = gameObject.AddComponent<LazyFollow>();
            }

            lazyFollow.target = Camera.main.transform;
            lazyFollow.targetOffset = offset;
            lazyFollow.rotationFollowMode = LazyFollow.RotationFollowMode.Follow;

            return;
        }

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
