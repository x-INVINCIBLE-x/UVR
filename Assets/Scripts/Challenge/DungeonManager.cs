#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(DungeonBuffHandler))]
public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance { get; private set; }
    private DungeonBuffHandler buffHandler;

    [Header("Difficulty Scaling")]
    [field: SerializeField] public int DifficultyLevel { get; private set; } = 1;
    [field: SerializeField] public int Level { get; private set; } = 1;
    [Tooltip("Levels to Complete before Difficultry Level Increase")]
    [field: SerializeField] private int levelsToComplete = 2;
    private int levelsTillScale = 0;

    [Header("Scene Transition")]
    [SerializeField] private SceneReference cityScene;
    [SerializeField] private DungeonDifficultyDatabase dungeonDifficultyDatabase;
    private SceneTransitionProvider transitionProvider;
    private List<SceneReference> currentDifficultyScenes;
    //[SerializeField] private SceneReference[] dungeonScenes;
    private int[] shuffledIndices;
    private int currentSceneIndex = 0;

    [SerializeField] private SceneReference[] dungeonSuccessTransitionScene;
    private int currentSuccessTransitionSceneIndex = 0;
    [SerializeField] private SceneReference[] dungeonFailTransitionScene;
    private int currentFailTransitionSceneIndex = 0;
    private const int Failure_Transition_Stay = 7;

    public event Action<int> OnDifficultyChange;

    #region Stats
    public int enemiesEliminatedLastRound = 0;
    public int goolsKilledLastRound = 0;
    public int shieldKilledLastRound = 0;
    public int muncherKilledLastRound = 0;
    public int batzKilledLastRound = 0;
    public int ironturetKilledLastRound = 0;
    public int turretsDestroyedLastRound = 0;
    #endregion
#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < dungeonFailTransitionScene.Length; i++)
        {
            dungeonFailTransitionScene[i].UpdateFields();
        }
        for (int i = 0; i < dungeonSuccessTransitionScene.Length; i++)
        {
            dungeonSuccessTransitionScene[i].UpdateFields();
        }
        cityScene.UpdateFields();
        EditorUtility.SetDirty(this);
    }
#endif

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        levelsTillScale = levelsToComplete;

        currentDifficultyScenes = dungeonDifficultyDatabase.GetScenesForDifficulty(DifficultyLevel);
        shuffledIndices = new int[currentDifficultyScenes.Count()];

        buffHandler = GetComponent<DungeonBuffHandler>();
        ResetAvailbleScenes();
        DifficultyLevel = 1;
        Level = 1;
    }

    private void Start()
    {
        buffHandler.OnBuffPick += GetHandleBuffSelection;

        if (GameManager.instance != null)
            GameManager.instance.enemiesEliminatedLastRound = 0;
    
        ChallengeManager challengeManager = ChallengeManager.instance;

        if (challengeManager != null)
        {
            ChallengeManager.instance.OnChallengeSuccess += HandleLevelCompletion;
            ChallengeManager.instance.OnChallengeFail += HandleLevelFailure;
        }

        GameEvents.OnElimination += HandleElimination;
    }

    public void HandleLevelCompletion()
    {
        transitionProvider.transform.root.gameObject.SetActive(true);
        buffHandler.Setup(transitionProvider.transform.root.transform, DifficultyLevel);

        currentSuccessTransitionSceneIndex = (currentSuccessTransitionSceneIndex + 1) % dungeonSuccessTransitionScene.Length;
        UpdateDifficulty();
    }

    public void HandleLevelFailure()
    {
        transitionProvider.Initialize(cityScene, 3f, Failure_Transition_Stay, dungeonFailTransitionScene[currentFailTransitionSceneIndex], true);
        transitionProvider.StartTransition();

        currentFailTransitionSceneIndex = (currentFailTransitionSceneIndex + 1) % dungeonFailTransitionScene.Length;

        if (GameManager.instance != null)
        {
            GameManager.instance.SetLastRoundData(ObjectiveType.JackOGools, goolsKilledLastRound);
            GameManager.instance.SetLastRoundData(ObjectiveType.GoblinShield, shieldKilledLastRound);
            GameManager.instance.SetLastRoundData(ObjectiveType.EmeraldBats, batzKilledLastRound);
            GameManager.instance.SetLastRoundData(ObjectiveType.FellironTurret, ironturetKilledLastRound);
            GameManager.instance.SetLastRoundData(ObjectiveType.Turret, turretsDestroyedLastRound);
            GameManager.instance.SetLastLevel(Level);
        }
    }

    private void UpdateDifficulty()
    {
        Level++;
        levelsTillScale--;

        if (levelsTillScale == 0)
        {
            DifficultyLevel++;
            levelsTillScale = levelsToComplete;
            OnDifficultyChange?.Invoke(DifficultyLevel);
            currentDifficultyScenes = dungeonDifficultyDatabase.GetScenesForDifficulty(DifficultyLevel);
            ResetAvailbleScenes();
        }
    }

    private void GetHandleBuffSelection()
    {
        if (currentSceneIndex >= shuffledIndices.Length)
            ResetAvailbleScenes();
        
        int sceneIndex = shuffledIndices[currentSceneIndex++];
        Debug.Log("buff Selectin handled");
        transitionProvider.Initialize(currentDifficultyScenes[sceneIndex], 1 , 5, dungeonSuccessTransitionScene[currentSuccessTransitionSceneIndex]);
        transitionProvider.transform.gameObject.SetActive(true);
    }

    private void ResetAvailbleScenes()
    {
        currentSceneIndex = 0;

        for (int i = 0; i < currentDifficultyScenes.Count; i++)
        {
            shuffledIndices[i] = i;
        }

        for (int i = shuffledIndices.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (shuffledIndices[i], shuffledIndices[j]) = (shuffledIndices[j], shuffledIndices[i]);
        }

    }

    public void RegisterSceneTransitionProvider(SceneTransitionProvider sceneTransitionProvider)
    {
        transitionProvider = sceneTransitionProvider;
        transitionProvider.transform.root.gameObject.SetActive(false);
    }

    private void HandleElimination(ObjectiveType type)
    {
        switch (type)
        {
            case ObjectiveType.JackOGools:
                goolsKilledLastRound++;
                break;
            case ObjectiveType.GoblinShield:
                shieldKilledLastRound++;
                break;
            case ObjectiveType.EmeraldBats:
                batzKilledLastRound++;
                break;
            case ObjectiveType.FellironTurret:
                ironturetKilledLastRound++;
                break;
            case ObjectiveType.Turret:
                turretsDestroyedLastRound++;
                break;
        }

        enemiesEliminatedLastRound++;
    }
    private void OnDestroy()
    {
        if (ChallengeManager.instance != null)
        {
            ChallengeManager.instance.OnChallengeSuccess -= HandleLevelCompletion;
            ChallengeManager.instance.OnChallengeFail -= HandleLevelFailure;
        }
    }
}