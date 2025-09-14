using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    public TypeofMesh meshType;

    [SerializeField] private MeshRenderer[] Meshes;
    [SerializeField] private SkinnedMeshRenderer[] SkinnedMeshes;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float materialUpTime = 3f;

    // cache original materials
    private Material[] defaultMaterials;

    public enum TypeofMesh
    {
        StaticMesh,
        SkinnedMesh
    }

    private void Awake()
    {
        switch (meshType)
        {
            case TypeofMesh.StaticMesh:
                StaticMeshInitialize();
                break;

            case TypeofMesh.SkinnedMesh:
                SkinnedMeshInitialize();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TerrainScanner2>() == null) return;

        switch (meshType)
        {
            case TypeofMesh.StaticMesh:
                StaticMeshHighlight();
                break;

            case TypeofMesh.SkinnedMesh:
                SkinnedMeshHighlight();
                break;
        }

        Invoke(nameof(ResetMaterial), materialUpTime);
    }

    // Initialization
    private void StaticMeshInitialize()
    {
        if (Meshes == null || Meshes.Length == 0)
            Meshes = GetComponentsInChildren<MeshRenderer>();

        int totalMats = 0;
        foreach (var r in Meshes) totalMats += r.sharedMaterials.Length;

        defaultMaterials = new Material[totalMats];

        int index = 0;
        foreach (var r in Meshes)
        {
            foreach (var mat in r.sharedMaterials)
            {
                defaultMaterials[index++] = mat;
            }
        }
    }

    private void SkinnedMeshInitialize()
    {
        if (SkinnedMeshes == null || SkinnedMeshes.Length == 0)
            SkinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();

        int totalMats = 0;
        foreach (var r in SkinnedMeshes) totalMats += r.sharedMaterials.Length;

        defaultMaterials = new Material[totalMats];

        int index = 0;
        foreach (var r in SkinnedMeshes)
        {
            foreach (var mat in r.sharedMaterials)
            {
                defaultMaterials[index++] = mat;
            }
        }
    }

    //Highlight
    private void StaticMeshHighlight()
    {
        foreach (var r in Meshes)
        {
            var newMats = new Material[r.sharedMaterials.Length];
            for (int j = 0; j < newMats.Length; j++)
                newMats[j] = highlightMaterial;

            r.materials = newMats;
        }
    }

    private void SkinnedMeshHighlight()
    {
        foreach (var r in SkinnedMeshes)
        {
            var newMats = new Material[r.sharedMaterials.Length];
            for (int j = 0; j < newMats.Length; j++)
                newMats[j] = highlightMaterial;

            r.materials = newMats;
        }
    }

    // Reset
    private void ResetMaterial()
    {
        int index = 0;

        switch (meshType)
        {
            case TypeofMesh.StaticMesh:
                foreach (var r in Meshes)
                {
                    var mats = new Material[r.sharedMaterials.Length];
                    for (int j = 0; j < mats.Length; j++)
                        mats[j] = defaultMaterials[index++];
                    r.materials = mats;
                }
                break;

            case TypeofMesh.SkinnedMesh:
                foreach (var r in SkinnedMeshes)
                {
                    var mats = new Material[r.sharedMaterials.Length];
                    for (int j = 0; j < mats.Length; j++)
                        mats[j] = defaultMaterials[index++];
                    r.materials = mats;
                }
                break;
        }
    }

    private void OnDisable()
    {
        ResetMaterial();
    }
}
