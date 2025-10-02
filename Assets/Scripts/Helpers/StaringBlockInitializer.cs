using System;
using UnityEngine;

public class StaringBlockInitializer : MonoBehaviour
{
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject shieldParent;

    private void Start()
    {
        ChallengeManager.instance.OnChallengeChoosen += InitializeStartingBlock;
    }

    private void InitializeStartingBlock(Challenge challenge)
    {
        if (shieldParent == null || shield == null) return;
        if (shieldParent.transform.childCount > 0) return;

        Instantiate(shield, shieldParent.transform);
    }

    private void OnDestroy()
    {
        ChallengeManager.instance.OnChallengeChoosen -= InitializeStartingBlock;
    }
}
