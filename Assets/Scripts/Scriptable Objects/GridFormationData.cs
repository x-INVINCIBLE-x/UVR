using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGridFormationData", menuName = "GridFormation/GridFormationData")]
public class GridFormationData : ScriptableObject
{
    public List<Vector3> positions;
}