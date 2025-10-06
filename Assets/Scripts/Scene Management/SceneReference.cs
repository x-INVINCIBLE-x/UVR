using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

[Serializable]
public class SceneReference
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;
#endif
    [SerializeField] private string scenePath;
    [SerializeField] private string sceneName;

    public string ScenePath => scenePath;
    public string SceneName => sceneName;
    public bool IsValid => !string.IsNullOrEmpty(scenePath) && !string.IsNullOrEmpty(sceneName);

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
