using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class FollowPlayer : MonoBehaviour
{
    private Transform playerTransform;

    [Header("Follow Settings")]
    [SerializeField] private bool useLazyFollow = false;
    [SerializeField] private bool rotateWithPlayer = false;
    [SerializeField] private bool followY = false;
    [SerializeField] private bool followPlayerOnce = false;
    [SerializeField] private Vector3 offset;

    private Vector3 followPosition;
    private bool hasFollowedOnce = false;

    private void OnEnable()
    {
        if (PlayerManager.instance != null)
        {
            playerTransform = PlayerManager.instance.Player.playerBody;
        }
    }

    private void Start()
    {
        if (useLazyFollow)
        {
            SetupLazyFollow();
            return;
        }

        if (playerTransform == null)
        {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (useLazyFollow) return;

        if (followPlayerOnce && hasFollowedOnce) return;

        if (playerTransform == null)
        {
            playerTransform = PlayerManager.instance.Player.playerBody;
        }

        UpdateFollowPosition();
        Follow();
        LookAt();

        if (followPlayerOnce)
            hasFollowedOnce = true;
    }

    private void UpdateFollowPosition()
    {
        followPosition = playerTransform.position
                         + playerTransform.right * offset.x
                         + playerTransform.up * offset.y
                         + playerTransform.forward * offset.z;

        if (!followY)
            followPosition.y = transform.position.y;
    }

    private void Follow()
    {
        transform.position = followPosition;
    }

    private void LookAt()
    {
        if (rotateWithPlayer)
        {
            transform.rotation = playerTransform.rotation;
        }
        else
        {
            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    private void SetupLazyFollow()
    {
        if (!TryGetComponent(out LazyFollow lazyFollow))
        {
            lazyFollow = gameObject.AddComponent<LazyFollow>();
        }

        lazyFollow.target = Camera.main.transform;
        lazyFollow.targetOffset = offset;
        lazyFollow.rotationFollowMode = LazyFollow.RotationFollowMode.Follow;
    }
}
