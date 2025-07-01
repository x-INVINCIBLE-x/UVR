#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.XR.CoreUtils;

public class SceneTransitionProvider : MonoBehaviour
{
    public static SceneTransitionProvider Instance;
    private const string Player_Tag = "Player";

    [Header("Scene References")]
    public SceneReference transitionScene;
    public SceneReference targetScene;

    [Header("Player")]
    public GameObject Core;

    [Header("UI")]
    public Fader fader;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


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
        DungeonManager.Instance.RegisterSceneTransitionProvider(this);
    }

    public void Initialize(SceneReference newTargetScene)
    {
        targetScene = newTargetScene;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Player_Tag))
        {
            StartCoroutine(TransitionRoutine());
        }
    }

    private IEnumerator TransitionRoutine()
    {
        // --- Fade out from current scene ---
        if (fader != null)
            yield return fader.FadeOut(1f);

        // --- Load transition scene ---
        // --- Remember the currently active scene ---
        Scene previousScene = SceneManager.GetActiveScene();

        // --- Load transition scene ---
        AsyncOperation loadTransition = SceneManager.LoadSceneAsync(transitionScene.SceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadTransition.isDone);

        Scene transitionLoadedScene = SceneManager.GetSceneByName(transitionScene.SceneName);
        SceneManager.MoveGameObjectToScene(Core, transitionLoadedScene);
        SceneManager.SetActiveScene(transitionLoadedScene);

        // --- Unload the previous scene now that transition is ready ---
        if (previousScene.name != transitionScene.SceneName)
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
            transitionLevelManager.ResetPlayerPosition();
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
        yield return new WaitForSeconds(5f);

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
            targetLevelManager.ResetPlayerPosition();
        }
        else
        {
            Debug.LogWarning("LevelManager or spawnPoint not found in target scene.");
        }

        // --- Final fade-in ---
        if (fader != null)
            yield return fader.FadeIn(1f);

        // --- Unload other scenes ---
        SceneManager.UnloadSceneAsync(transitionScene.SceneName);

        // --- Cleanup ---
        gameObject.SetActive(false);
    }
}
