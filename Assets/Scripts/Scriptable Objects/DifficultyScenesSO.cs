using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Dungeon Difficulty Level")]
public class DifficultyScenesSO : ScriptableObject
{
    public int difficulty;
    public List<SceneReference> scenes;

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < scenes.Count; i++)
        {
            scenes[i].UpdateFields();
        }
    }
#endif
}