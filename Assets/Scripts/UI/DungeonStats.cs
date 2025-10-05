using TMPro;
using UnityEngine;

public class DungeonStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lastAttemptDepth;
    [SerializeField] private TextMeshProUGUI enemiesKilledLastAttempt;

    [SerializeField] private TextMeshProUGUI highestDepthReached;
    [SerializeField] private TextMeshProUGUI totalEnemiesKilled;
    [SerializeField] private TextMeshProUGUI totalAttempts;

    private void Start()
    {
        if (GameManager.instance != null)
        {
            lastAttemptDepth.text = GameManager.instance.lastDepth.ToString();
            enemiesKilledLastAttempt.text = GameManager.instance.enemiesEliminatedLastRound.ToString();
            highestDepthReached.text = GameManager.instance.highestDepthReached.ToString();
            totalEnemiesKilled.text = GameManager.instance.totalEnemiesKilled.ToString();
            totalAttempts.text = GameManager.instance.totalAttempts.ToString();
        }
    }
}
