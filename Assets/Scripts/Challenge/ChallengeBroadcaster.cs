using UnityEngine;

public class ChallengeBroadcaster : MonoBehaviour
{
    public void Start()
    {
        ChallengeManager.instance.RegisterShield(gameObject);
    }
}
