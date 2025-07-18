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

    [field: SerializeField] public int DifficultyLevel { get; private set; } = 1;
    [Tooltip("Levels to Complete before Difficultry Level Increase")]
    [field: SerializeField] private int levelsToComplete = 2;
    private int levelsTillScale = 0;

    [SerializeField] private SceneReference cityScene;
    [SerializeField] private SceneTransitionProvider transitionProvider;

    [SerializeField] private SceneReference[] dungeonScenes;
    private int[] shuffledIndices;
    private int currentSceneIndex = 0;

    [SerializeField] private SceneReference[] dungeonFailTransitionScene;
    private int currentFailTransitionSceneIndex = 0;
    private const int Failure_Transition_Stay = 7;

    public event Action<int> OnDifficultyChange;

    //IDK if it belongs here????
    [SerializeField] private GameObject endGround;
    //public GameObject tempEnemy;

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < dungeonScenes.Length; i++)
        {
            dungeonScenes[i].UpdateFields();
        }
        for (int i = 0; i < dungeonFailTransitionScene.Length; i++)
        {
            dungeonFailTransitionScene[i].UpdateFields();
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

        shuffledIndices = new int[dungeonScenes.Count()];

        buffHandler = GetComponent<DungeonBuffHandler>();
        ResetAvailbleScenes();
        //DifficultyLevel = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            DifficultyLevel++;
            //Instantiate(tempEnemy, transform.position, Quaternion.identity);
            OnDifficultyChange?.Invoke(DifficultyLevel);
        }
    }

    private void Start()
    {
        //endGround.SetActive(false);

        buffHandler.OnBuffPick += GetHandleBuffSelection;

        ChallengeManager challengeManager = ChallengeManager.instance;

        if (challengeManager != null)
        {
            ChallengeManager.instance.OnChallengeSuccess += HandleLevelCompletion;
            ChallengeManager.instance.OnChallengeFail += HandleLevelFailure;
        }
    }

    public void HandleLevelCompletion()
    {
        endGround.SetActive(true);
        buffHandler.Setup(endGround.transform, DifficultyLevel);

        UpdateDifficulty();
    }

    public void HandleLevelFailure()
    {
        transitionProvider.Initialize(cityScene, 3f, Failure_Transition_Stay, dungeonFailTransitionScene[currentFailTransitionSceneIndex]);
        transitionProvider.StartTransition();

        currentFailTransitionSceneIndex = (currentFailTransitionSceneIndex + 1) % dungeonFailTransitionScene.Length;
    }

    private void UpdateDifficulty()
    {
        levelsTillScale--;

        if (levelsTillScale == 0)
        {
            DifficultyLevel++;
            levelsTillScale = levelsToComplete;
            OnDifficultyChange?.Invoke(DifficultyLevel);
        }
    }

    private void GetHandleBuffSelection()
    {
        if (currentSceneIndex >= shuffledIndices.Length)
            ResetAvailbleScenes();

        int sceneIndex = shuffledIndices[currentSceneIndex++];

        transitionProvider.Initialize(dungeonScenes[sceneIndex]);
        transitionProvider.gameObject.SetActive(true);
    }

    private void ResetAvailbleScenes()
    {
        currentSceneIndex = 0;

        for (int i = 0; i < dungeonScenes.Length; i++)
        {
            shuffledIndices[i] = i;
        }

        for (int i = shuffledIndices.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (shuffledIndices[i], shuffledIndices[j]) = (shuffledIndices[j], shuffledIndices[i]);
        }

    }
    public void RegisterSceneTransitionProvider(SceneTransitionProvider sceneTransitionProvider) =>
        transitionProvider = sceneTransitionProvider;

    private void OnDestroy()
    {
        if (ChallengeManager.instance != null)
        {
            ChallengeManager.instance.OnChallengeSuccess -= HandleLevelCompletion;
            ChallengeManager.instance.OnChallengeFail -= HandleLevelFailure;
        }
    }
}