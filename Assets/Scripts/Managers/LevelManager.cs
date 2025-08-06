using UnityEngine;
using Unity.XR.CoreUtils;

public class LevelManager : MonoBehaviour
{
    public Transform spawnTransform;

    public void SetPlayerToSpawnPosition()
    {
        PlayerManager.instance.SetPlayerPosition(spawnTransform.position);
    }
}
