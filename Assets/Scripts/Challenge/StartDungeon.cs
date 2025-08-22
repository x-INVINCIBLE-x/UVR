using UnityEngine;
using UnityEngine.SceneManagement;

public class StartDungeon : MonoBehaviour
{
    [SerializeField] private GameObject dungeonEssentials;
    [SerializeField] private DynamicFormationController groundGrid;

    void Start()
    {
        GameObject core = FindRootCore();
        if (core != null)
        {
            Instantiate(dungeonEssentials, core.transform);
        }
        else
            Debug.Log("Core not found in root objects.");
    }

    GameObject FindRootCore()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = currentScene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name == "Core")
                return obj;
        }

        return null;
    }
}
