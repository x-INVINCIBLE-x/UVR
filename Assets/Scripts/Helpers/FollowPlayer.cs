using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;
using static UnityEngine.GraphicsBuffer;

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

    private void OnEnable()
    {
        if (PlayerManager.instance != null)
        {
            followPosition = PlayerManager.instance.Player.playerBody.position + offset;
            LookAt();
        }
    }

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
        LookAt();
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

        followPosition = playerTransform.position
                         + playerTransform.right * offset.x
                         + playerTransform.up * offset.y
                         + playerTransform.forward * offset.z;

        if (!followY)
        {
            followPosition.y = transform.position.y;
        }

        Follow();
        LookAt();
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
            Vector3 lookDirection = playerTransform.position - transform.position;
            lookDirection.y = 0; // Keep the Y component zero to avoid tilting
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }
}
