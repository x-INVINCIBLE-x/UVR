using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Serialization;

public class SimplifyMesh : MonoBehaviour
{
    public float quality = 0.5f;

    private void OnGUI()
    {
        if (GUI.Button(Rect.MinMaxRect(0, 0, 200, 200), "SimplifyMesh"))
        {
            var originalMesh = GetComponent<MeshFilter>().sharedMesh;
            var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
            meshSimplifier.Initialize(originalMesh);
            meshSimplifier.SimplifyMesh(quality);
            var destMesh = meshSimplifier.ToMesh();
            GetComponent<MeshFilter>().sharedMesh = destMesh;
        }
    }
}