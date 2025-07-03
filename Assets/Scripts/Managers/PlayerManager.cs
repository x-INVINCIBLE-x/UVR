using Unity.XR.CoreUtils;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    [field: SerializeField] public Player Player {  get; private set; }
    [field: SerializeField] public Rigidbody Rb { get; private set; }
    [field: SerializeField] public XROrigin PlayerOrigin { get; private set; }
    [field: SerializeField] public ActionMediator ActionMediator { get; private set; }

    public event System.Action OnPlayerDeath
    {
        add => Player.stats.OnPlayerDeath += value;
        remove => Player.stats.OnPlayerDeath -= value;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetPlayerPosition(Transform targetTransform)
    {
        PlayerOrigin.MoveCameraToWorldLocation(targetTransform.position);
        PlayerOrigin.MatchOriginUpCameraForward(targetTransform.up, targetTransform.forward);
    }

    public void SetPlayerPosition(Vector3 targetPosition)
    {
        PlayerOrigin.MoveCameraToWorldLocation(targetPosition);
        PlayerOrigin.MatchOriginUpCameraForward(Vector3.up, Vector3.forward);
    }
}
