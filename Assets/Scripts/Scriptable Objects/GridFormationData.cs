using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "GridFormationData", menuName = "Grid/GridFormationData")]
public class GridFormationData : ScriptableObject
{
    public List<Vector3> positions;
    public List<Quaternion> rotations;
    public List<GameObject> prefabs;
}
