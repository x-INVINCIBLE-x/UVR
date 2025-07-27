using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Difficulty Database")]
public class DungeonDifficultyDatabase : ScriptableObject
{
    public List<DifficultyScenesSO> levels;

    public List<SceneReference> GetScenesForDifficulty(int difficulty)
    {
        DifficultyScenesSO level = levels.Find(l => l.difficulty == difficulty);
        return level == null ? null : level.scenes;
    }
}
