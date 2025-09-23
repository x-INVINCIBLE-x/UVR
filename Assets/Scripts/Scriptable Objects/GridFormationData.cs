using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridFormationData : ScriptableObject
{
    public List<Vector3> positions = new List<Vector3>();
    public List<Quaternion> rotations = new List<Quaternion>(); 
}
