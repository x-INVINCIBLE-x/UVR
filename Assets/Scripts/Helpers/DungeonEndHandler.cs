using System;
using UnityEngine;

public class DungeonEndHandler : MonoBehaviour
{
    private void Start()
    {
        ChallengeManager.instance.OnChallengeFail += HandleDungeonEnd;
    }

    private void HandleDungeonEnd()
    {
        Destroy(gameObject, 5f);
    }

    private void OnDestroy()
    {
        if (ChallengeManager.instance != null)
        {
            ChallengeManager.instance.OnChallengeFail -= HandleDungeonEnd;
        }
    }
}
