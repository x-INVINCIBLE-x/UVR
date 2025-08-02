using UnityEngine;

public class RecreateDungeon : MonoBehaviour
{
    private void Start()
    {
        foreach (var grid in FormationHandler.Instance.gridSpawners)
        {
            if (grid != null)
            {
                grid.ReloadRandomFormation();
            }
        }
    }
}
