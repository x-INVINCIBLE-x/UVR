using System.Collections;
using UnityEngine;

public class ChallengeBroadcaster : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(RegisterSelf());
    }

    IEnumerator RegisterSelf()
    {
        yield return new WaitUntil(() => ChallengeManager.instance != null);
        ChallengeManager.instance.RegisterShield(gameObject);
    }
}
