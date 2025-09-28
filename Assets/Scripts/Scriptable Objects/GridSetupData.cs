using UnityEngine;

[CreateAssetMenu(fileName = "GridSetupData", menuName = "GridFormation/GridSetupData", order = 1)]
public class GridSetupData : ScriptableObject
{
    public GridFormationController[] gridFormationControllers;
    public Vector3[] positions;
    public Quaternion[] rotations;

    public Vector3 safePosition;
    public Vector3 safeRotation;
    public Vector2 safeArea;

    [ContextMenu("Save Transforms")]
    public void SaveTransforms()
    {
        if (gridFormationControllers.Length > 0)
        {
            positions = new Vector3[gridFormationControllers.Length];
            rotations = new Quaternion[gridFormationControllers.Length];
            for (int i = 0; i < gridFormationControllers.Length; i++)
            {
                if (gridFormationControllers[i] != null)
                {
                    positions[i] = gridFormationControllers[i].transform.position;
                    rotations[i] = gridFormationControllers[i].transform.rotation;
                }
            }
        }
    }
}