using UnityEngine;
using Unity.XR.CoreUtils;

public class LevelManager : MonoBehaviour
{
    public XROrigin playerOrigin;
    public Transform spawnTransform;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetPlayerPosition();
        }
    }

    public void ResetPlayerPosition()
    {
        playerOrigin.MoveCameraToWorldLocation(spawnTransform.position);
        playerOrigin.MatchOriginUpCameraForward(spawnTransform.up, spawnTransform.forward);
    }
}
