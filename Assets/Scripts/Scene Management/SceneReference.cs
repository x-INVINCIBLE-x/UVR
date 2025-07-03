using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

[Serializable]
public class SceneReference
{
    [SerializeField] private SceneAsset sceneAsset;
    [SerializeField] private string scenePath;
    [SerializeField] private string sceneName;

    public string ScenePath => scenePath;
    public string SceneName => sceneName;
    public bool HasSceneAsset => sceneAsset != null;

#if UNITY_EDITOR
    public void UpdateFields()
    {
        if (sceneAsset != null)
        {
            scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }
        else
        {
            scenePath = "";
            sceneName = "";
        }
    }
#endif
}
