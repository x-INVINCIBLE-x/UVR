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

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartCoroutine(TransitionRoutine());
        }
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

        if (Core != null)
        {
            Debug.Log("Found");
        }
        else
            Debug.Log("Core not found in root objects.");
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
        // --- Fade out from current scene ---
        if (fader != null)
            yield return fader.FadeOut(fadeInTime);

        // --- Load transition scene ---
        // --- Remember the currently active scene ---
        Scene previousScene = SceneManager.GetActiveScene();
        SceneReference currentTransitionScene = providedTransitionScene.HasSceneAsset ? providedTransitionScene : transitionScene;

        // --- Load transition scene ---
        AsyncOperation loadTransition = SceneManager.LoadSceneAsync(currentTransitionScene.SceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadTransition.isDone);

        Scene transitionLoadedScene = SceneManager.GetSceneByName(currentTransitionScene.SceneName);

        SceneManager.MoveGameObjectToScene(Core, transitionLoadedScene);
        SceneManager.SetActiveScene(transitionLoadedScene);

        // --- Unload the previous scene now that transition is ready ---
        if (previousScene.name != currentTransitionScene.SceneName)
        {
            SceneManager.UnloadSceneAsync(previousScene);
        }

        // --- Set Core spawn in transition scene ---
        LevelManager transitionLevelManager = null;
        foreach (GameObject root in transitionLoadedScene.GetRootGameObjects())
        {
            transitionLevelManager = root.GetComponentInChildren<LevelManager>();
            if (transitionLevelManager != null)
                break;
        }

        if (transitionLevelManager != null)
        {
            transitionLevelManager.SetPlayerToSpawnPosition();
        }
        else
        {
            Debug.LogWarning("LevelManager or spawnPoint not found in transition scene.");
        }

        // Fade in to reveal transition scene (optional)
        if (fader != null)
            yield return fader.FadeIn(1f);

        // --- Load target scene ---
        AsyncOperation loadTarget = SceneManager.LoadSceneAsync(targetScene.SceneName, LoadSceneMode.Additive);
        loadTarget.allowSceneActivation = false;

        // Wait until loading is *almost* done
        yield return new WaitUntil(() => loadTarget.progress >= 0.9f);

        // Optional delay (e.g., play animation)
        yield return new WaitForSeconds(stayDuraion);

        // Optional: fade, animation, input, etc.
        if (fader != null)
            yield return fader.FadeOut(1f);

        // Allow activation now
        loadTarget.allowSceneActivation = true;

        // Wait until scene fully activates
        yield return new WaitUntil(() => loadTarget.isDone);

        // --- Fade out to load target scene ---
        if (fader != null)
            yield return fader.FadeOut(1f);

        Scene targetLoadedScene = SceneManager.GetSceneByName(targetScene.SceneName);
        SceneManager.MoveGameObjectToScene(Core, targetLoadedScene);
        SceneManager.SetActiveScene(targetLoadedScene);

        // --- Set Core spawn in target scene ---
        LevelManager targetLevelManager = null;
        foreach (GameObject root in targetLoadedScene.GetRootGameObjects())
        {
            targetLevelManager = root.GetComponentInChildren<LevelManager>();
            if (targetLevelManager != null)
                break;
        }

        if (targetLevelManager != null)
        {
            targetLevelManager.SetPlayerToSpawnPosition();
        }
        else
        {
            Debug.LogWarning("LevelManager or spawnPoint not found in target scene.");
        }

        // --- Final fade-in ---
        if (fader != null)
            yield return fader.FadeIn(1f);

        // --- Unload other scenes ---
        SceneManager.UnloadSceneAsync(currentTransitionScene.SceneName);

        Destroy(gameObject);
    }

    GameObject FindRootCore()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = currentScene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name == "Core")
            {
                Debug.Log("oy=nd");
                return obj;
            }

        }

        Debug.Log("Cant find core in " + currentScene.name);
        return null;
    }
}
