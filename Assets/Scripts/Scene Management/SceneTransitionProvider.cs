#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.XR.CoreUtils;
using NUnit.Framework.Internal.Builders;

public class SceneTransitionProvider : MonoBehaviour
{
    private const string Player_Tag = "Player";

    [Header("Scene References")]
    public SceneReference transitionScene;
    public SceneReference targetScene;
    public SceneReference providedTransitionScene = null;

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

    public void Initialize(SceneReference newTargetScene, float fadeInTime = 1f, int transitionStayDuration = 5, SceneReference newTransitionReference = null)
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
    }

    public void StartTransition()
    {
        DontDestroyOnLoad(gameObject);
        currentTransitionRoutine ??= CoroutineManager.instance.StartCoroutine(TransitionRoutine());

        //StartCoroutine(TransitionRoutine());
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

        AsyncOperation loadTarget = SceneManager.LoadSceneAsync(targetScene.SceneName, LoadSceneMode.Additive);
        loadTarget.allowSceneActivation = false;
        yield return new WaitUntil(() => loadTarget.progress >= 0.9f);

        yield return new WaitForSeconds(stayDuraion);
        yield return HandleFadeOut();

        loadTarget.allowSceneActivation = true;
        yield return new WaitUntil(() => loadTarget.isDone);

        Scene targetSceneObj = SceneManager.GetSceneByName(targetScene.SceneName);
        SceneManager.MoveGameObjectToScene(Core, targetSceneObj);
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
        SceneManager.MoveGameObjectToScene(Core, targetSceneObj);
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
}
