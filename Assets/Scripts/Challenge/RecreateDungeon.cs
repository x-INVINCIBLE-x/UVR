using UnityEngine;

public class RecreateDungeon : MonoBehaviour
{
    private void Start()
    {
        foreach (var grid in FormationHandler.Instance.gridSpawners)
        {
            if (grid != null)
            {
                string ChallengeID = ChallengeManager.instance.CurrentChallenge.GetID();
                Debug.Log($"Recreating dungeon for Challenge ID: {ChallengeID}");
                grid.ReloadRandomFormation(ChallengeID);
            }
        }
    }
}
