#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.XR.CoreUtils;

public class SceneTransitionProvider : MonoBehaviour
{
    private const string Player_Tag = "Player";

    [Header("Scene References")]
    public SceneReference transitionScene;
    public SceneReference targetScene;
    public SceneReference providedTransitionScene = null;

    [Header("Priority Scene")]
    public SceneReference priorityScene;
    private static bool isPrioritySceneLoaded = false;
    public static string prioritySceneName;

    [Header("Player")]
    public GameObject Core;

    [Header("UI")]
    public Fader fader;

    private float fadeInTime = 1f;
    private int stayDuraion = 5;
    private Coroutine currentTransitionRoutine = null;

#if UNITY_EDITOR
    private void OnValidate()
    {
        transitionScene?.UpdateFields();
        targetScene?.UpdateFields();
        priorityScene?.UpdateFields();
        EditorUtility.SetDirty(this);
    }
#endif

    private void Start()
    {
        currentTransitionRoutine = null;
        if (DungeonManager.Instance != null)
        {
            DungeonManager.Instance.RegisterSceneTransitionProvider(this);
            gameObject.SetActive(false);
        }

        Core = FindRootCore();

        if (Core != null && fader == null)
            fader = Core.GetComponentInChildren<Fader>();
    }

    public void Initialize(SceneReference newTargetScene, float fadeInTime = 1f, int transitionStayDuration = 5, SceneReference newTransitionReference = null, bool removePriorityScene = false)
    {
        targetScene = newTargetScene;

        if (newTransitionReference != null)
        {
            providedTransitionScene = newTransitionReference;
        }
        else
        {
            providedTransitionScene = null;
        }

        this.fadeInTime = fadeInTime;
        stayDuraion = transitionStayDuration;

        if (Core == null)
            Core = FindRootCore();

        if (fader == null)
            fader = Core.GetComponentInChildren<Fader>();

        if (removePriorityScene)
        {
            if (isPrioritySceneLoaded)
            {
                SceneManager.UnloadSceneAsync(priorityScene.SceneName);
                isPrioritySceneLoaded = false;
                Debug.Log("Priority Scene Unloaded");
            }
        }
        }

    public void StartTransition()
    {
        DontDestroyOnLoad(gameObject);
        currentTransitionRoutine ??= CoroutineManager.instance.StartCoroutine(TransitionRoutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Player_Tag))
        {
            StartTransition();
        }
    }

    private IEnumerator TransitionRoutine()
    {
        yield return HandleFadeOut();

        Scene previousScene = SceneManager.GetActiveScene();
        SceneReference currentTransitionScene = (providedTransitionScene != null && providedTransitionScene.IsValid)
            ? providedTransitionScene
            : (transitionScene != null && transitionScene.IsValid ? transitionScene : null);

        if (currentTransitionScene != null)
        {
            yield return TransitionWithIntermediate(previousScene, currentTransitionScene);
        }
        else
        {
            yield return TransitionDirectly(previousScene);
        }
    }

    private IEnumerator TransitionWithIntermediate(Scene previousScene, SceneReference transitionSceneRef)
    {
        yield return LoadAndEnterScene(transitionSceneRef);

        if (previousScene.name != transitionSceneRef.SceneName)
            yield return UnloadScene(previousScene.name);

        yield return HandleFadeIn();

        // Load priority scene AFTER transition scene is active
        if (priorityScene != null && priorityScene.IsValid && !isPrioritySceneLoaded)
        {
            AsyncOperation loadPriority = SceneManager.LoadSceneAsync(priorityScene.SceneName, LoadSceneMode.Additive);
            loadPriority.allowSceneActivation = true;
            yield return new WaitUntil(() => loadPriority.isDone);
            isPrioritySceneLoaded = true;
            prioritySceneName = priorityScene.SceneName;
            Debug.Log("Priority Scene Loaded (after transition)");
        }

        // Wait for priority scene to be ready
        if (isPrioritySceneLoaded)
        {
            yield return new WaitUntil(() =>
                PrioritySceneGate.Instance != null && PrioritySceneGate.Instance.IsReady);
            Debug.Log("Priority Scene Ready");
        }

        AsyncOperation loadTarget = SceneManager.LoadSceneAsync(targetScene.SceneName, LoadSceneMode.Additive);
        loadTarget.allowSceneActivation = false;
        yield return new WaitUntil(() => loadTarget.progress >= 0.9f);

        yield return new WaitForSeconds(stayDuraion);
        yield return HandleFadeOut();

        loadTarget.allowSceneActivation = true;
        yield return new WaitUntil(() => loadTarget.isDone);

        Scene targetSceneObj = SceneManager.GetSceneByName(targetScene.SceneName);

        // Move Core to priority if available
        if (isPrioritySceneLoaded)
        {
            Scene prioScene = SceneManager.GetSceneByName(prioritySceneName);
            //Debug.Log($"Moving Core to priority scene: {prioScene.name}");
            //if (prioScene != null && prioScene != default)
                SceneManager.MoveGameObjectToScene(Core, prioScene);
        }
        else
        {
            SceneManager.MoveGameObjectToScene(Core, targetSceneObj);
        }

        SceneManager.SetActiveScene(targetSceneObj);

        LevelManager lm = GetLevelManagerFromScene(targetSceneObj);
        if (lm != null)
            lm.SetPlayerToSpawnPosition();

        yield return HandleFadeIn();
        
        yield return UnloadScene(transitionSceneRef.SceneName);
    }


    private IEnumerator TransitionDirectly(Scene previousScene)
    {
        AsyncOperation loadTarget = SceneManager.LoadSceneAsync(targetScene.SceneName, LoadSceneMode.Additive);
        loadTarget.allowSceneActivation = false;
        yield return new WaitUntil(() => loadTarget.progress >= 0.9f);

        yield return new WaitForSeconds(0.1f);
        yield return HandleFadeOut();

        loadTarget.allowSceneActivation = true;
        yield return new WaitUntil(() => loadTarget.isDone);

        Scene targetSceneObj = SceneManager.GetSceneByName(targetScene.SceneName);

        // Move Core to priority if loaded, else target
        if (isPrioritySceneLoaded)
        {
            Scene prioScene = SceneManager.GetSceneByName(priorityScene.SceneName);
            SceneManager.MoveGameObjectToScene(Core, prioScene);
        }
        else
        {
            SceneManager.MoveGameObjectToScene(Core, targetSceneObj);
        }

        SceneManager.SetActiveScene(targetSceneObj);

        LevelManager lm = GetLevelManagerFromScene(targetSceneObj);
        if (lm != null)
            lm.SetPlayerToSpawnPosition();

        yield return HandleFadeIn();

        yield return UnloadScene(previousScene.name);
    }

    private IEnumerator LoadAndEnterScene(SceneReference sceneRef)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneRef.SceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadOp.isDone);

        Scene loadedScene = SceneManager.GetSceneByName(sceneRef.SceneName);
        SceneManager.MoveGameObjectToScene(Core, loadedScene);
        SceneManager.SetActiveScene(loadedScene);

        LevelManager lm = GetLevelManagerFromScene(loadedScene);
        if (lm != null)
            lm.SetPlayerToSpawnPosition();
        else
            Debug.LogWarning($"LevelManager or spawnPoint not found in scene: {sceneRef.SceneName}");
    }

    private IEnumerator UnloadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
            SceneManager.UnloadSceneAsync(sceneName);
        yield return null;
    }

    private LevelManager GetLevelManagerFromScene(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            LevelManager lm = root.GetComponentInChildren<LevelManager>();
            if (lm != null)
                return lm;
        }
        return null;
    }

    private IEnumerator HandleFadeOut()
    {
        if (fader != null)
            yield return fader.FadeOut(fadeInTime);
    }

    private IEnumerator HandleFadeIn()
    {
        if (fader != null)
            yield return fader.FadeIn(1f);
    }

    GameObject FindRootCore()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = currentScene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name == "Core")
            {
                return obj;
            }
        }

        Debug.Log("Cant find core in " + currentScene.name);
        return null;
    }

    public void UnloadPriorityScene()
    {
        if (isPrioritySceneLoaded)
        {
            SceneManager.UnloadSceneAsync(priorityScene.SceneName);
            isPrioritySceneLoaded = false;
            prioritySceneName = string.Empty;
        }
    }
}