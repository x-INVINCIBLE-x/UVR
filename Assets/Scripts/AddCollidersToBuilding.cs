using UnityEngine;

public class AddCollidersToBuilding : MonoBehaviour
{
    void Start()
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in meshRenderers)
        {
            GameObject obj = renderer.gameObject;
            if (obj.GetComponent<Collider>() == null) // Avoid adding duplicate colliders
            {
                MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
                    meshCollider.convex = false; // Set to true only if needed
                }
            }
        }
        Debug.Log("Colliders added to all Mesh Renderers in the building model.");
    }
}

