using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshPrefabBaker : MonoBehaviour
{
    public NavMeshData[] navMeshData;
    private NavMeshSurface[] surfaces;
    private NavMeshDataInstance[] instances;

    void Awake()
    {
        surfaces = GetComponents<NavMeshSurface>();
        instances = new NavMeshDataInstance[surfaces.Length];
        navMeshData = new NavMeshData[surfaces.Length];

        for (int i = 0; i < surfaces.Length; i++)
        {
            var surface = surfaces[i];

            if (surface.navMeshData != null)
            {
                instances[i] = NavMesh.AddNavMeshData(surface.navMeshData, transform.position, transform.rotation);
                navMeshData[i] = surface.navMeshData;
            }
            else
            {
                Debug.LogWarning($"{name}: Surface {i} has no NavMeshData.");
            }
        }
    }

    void OnDestroy()
    {
        foreach (var instance in instances)
        {
            instance.Remove();
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Bake and Save All NavMeshes")]
    public void BakeNavMeshes()
    {
        surfaces = GetComponents<NavMeshSurface>();
        navMeshData = new NavMeshData[surfaces.Length];

        string folderPath = "Assets/NavMeshes";
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets", "NavMeshes");

        for (int i = 0; i < surfaces.Length; i++)
        {
            var surface = surfaces[i];

            surface.BuildNavMesh();

            if (surface.navMeshData == null)
            {
                Debug.LogError($"NavMesh build failed for surface {i}.");
                continue;
            }

            string assetPath = $"{folderPath}/{gameObject.name}_NavMesh_{i}.asset";
            AssetDatabase.CreateAsset(surface.navMeshData, assetPath);
            AssetDatabase.SaveAssets();

            navMeshData[i] = surface.navMeshData;
            Debug.Log($"NavMesh {i} baked and saved to: {assetPath}");
        }
    }
#endif
}
