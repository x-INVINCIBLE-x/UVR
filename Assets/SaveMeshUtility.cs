using UnityEngine;
using UnityEditor;

public class SaveMeshUtility
{
    //[MenuItem("Tools/Save Selected Mesh")]
    //public static void SaveSelectedMesh()
    //{
    //    var mf = Selection.activeGameObject?.GetComponent<MeshFilter>();
    //    if (mf == null)
    //    {
    //        Debug.LogWarning("No MeshFilter selected!");
    //        return;
    //    }

    //    var mesh = mf.sharedMesh;
    //    if (mesh == null)
    //    {
    //        Debug.LogWarning("No mesh found on selected object!");
    //        return;
    //    }

    //    string path = $"Assets/Simplified Mesh/SavedMesh_{mf.gameObject.name}.asset";
    //    AssetDatabase.CreateAsset(Object.Instantiate(mesh), path);
    //    AssetDatabase.SaveAssets();
    //    AssetDatabase.Refresh();
    //    Debug.Log("Mesh saved at: " + path);
    //}
}
