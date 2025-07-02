using UnityEngine;
using Unity.XR.CoreUtils;

public class LevelManager : MonoBehaviour
{
    public XROrigin playerOrigin;
    public Transform spawnTransform;

    public void ResetPlayerPosition()
    {
        if (playerOrigin == null)
            playerOrigin = PlayerManager.instance.playerOrigin;

        SetPlayerPosition(spawnTransform);
    }

    public void SetPlayerPosition(Transform targetTransform)
    {
        playerOrigin.MoveCameraToWorldLocation(targetTransform.position);
        playerOrigin.MatchOriginUpCameraForward(targetTransform.up, targetTransform.forward);
    }
}
