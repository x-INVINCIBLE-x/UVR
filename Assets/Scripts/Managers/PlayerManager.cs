using System.Collections;
using TMPro;
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
        add => Player.Stats.OnDeath += value;
        remove => Player.Stats.OnDeath -= value;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetPlayerPosition(Vector3 targetPosition)
    {
        StartCoroutine(SetPlayerPositionDelayed(targetPosition));
    }

    private IEnumerator SetPlayerPositionDelayed(Vector3 targetPosition)
    {
        bool kinematicState = Rb.isKinematic;
        Rb.isKinematic = true; // Disable physics temporarily
        Debug.Log("Setting player position to: " + targetPosition);
        PlayerOrigin.MoveCameraToWorldLocation(targetPosition);
        yield return null; 
        PlayerOrigin.MatchOriginUpCameraForward(Vector3.up, Vector3.forward);
        Rb.isKinematic = kinematicState; 
    }
}
