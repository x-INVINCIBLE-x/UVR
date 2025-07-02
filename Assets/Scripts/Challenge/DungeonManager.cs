#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

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

    [Header("Buff")]
    public List<BuffGroup> buffGroups = new List<BuffGroup>();
    private Dictionary<int, Dictionary<BuffCategory, List<Buff>>> buffLookup = new();
    private Dictionary<(int, BuffCategory), HashSet<Buff>> shownBuffs = new();

    public event Action<int> OnDifficultyChange;
    public GameObject tempEnemy;

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
        DifficultyLevel = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            DifficultyLevel++;
            Instantiate(tempEnemy, transform.position, Quaternion.identity);
            OnDifficultyChange?.Invoke(DifficultyLevel);
        }
    }

    private void Start()
    {
        buffHandler.OnBuffPick += GetHandleBuffSelection;

        foreach (BuffGroup group in buffGroups)
        {
            if (!buffLookup.ContainsKey(group.difficultyLevel))
                buffLookup[group.difficultyLevel] = new Dictionary<BuffCategory, List<Buff>>();

            foreach (var buffBind in group.buffsBind)
            {
                buffLookup[group.difficultyLevel][buffBind.category] = buffBind.buffs;
            }
        }
    }

    public void HandleLevelCompletion()
    {
        System.Random rng = new System.Random();
        int possibleChoices = Enum.GetValues(typeof(BuffCategory)).Length;
        List<int> numbers = Enumerable.Range(0, possibleChoices).OrderBy(x => rng.Next()).Take(3).ToList();

        for (int i = 0; i < buffHandler.buffProviders.Count; i++)
        {
            int choice = numbers[i];
            var availableBuffs = GetAvailableBuffs((BuffCategory)choice);
            Buff buffToProvide = availableBuffs[UnityEngine.Random.Range(0, availableBuffs.Count)];

            buffHandler.ProvideBuffs(i, buffToProvide, (BuffCategory)choice);

            MarkBuffAsUsed((BuffCategory)choice, buffToProvide);
        }

        UpdateDifficulty();
    }

    public void HandleLevelFailure()
    {
        transitionProvider.Initialize(cityScene, Failure_Transition_Stay, dungeonFailTransitionScene[currentFailTransitionSceneIndex]);
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


    public List<Buff> GetAvailableBuffs(BuffCategory category)
    {
        if (!buffLookup.TryGetValue(DifficultyLevel, out Dictionary<BuffCategory, List<Buff>> categoryDict) ||
            !categoryDict.TryGetValue(category, out List<Buff> allBuffs))
        {
            Debug.LogWarning($"No buffs found for difficulty {DifficultyLevel} and category {category}");
            return new List<Buff>();
        }

        var key = (DifficultyLevel, category);

        if (!shownBuffs.ContainsKey(key))
            shownBuffs[key] = new HashSet<Buff>();

        var used = shownBuffs[key];

        var unshownBuffs = allBuffs.Where(buff => !used.Contains(buff)).ToList();

        if (unshownBuffs.Count == 0)
        {
            used.Clear();
            unshownBuffs = new List<Buff>(allBuffs);
        }

        return unshownBuffs;
    }

    public void MarkBuffAsUsed(BuffCategory category, Buff selectedBuff)
    {
        var key = (DifficultyLevel, category);

        if (!shownBuffs.ContainsKey(key))
            shownBuffs[key] = new HashSet<Buff>();

        shownBuffs[key].Add(selectedBuff);
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

    public void RegisterBuffPortal(DungeonBuffProvider buffProvider) => buffHandler.AddBuffProvider(buffProvider);
    public void RegisterSceneTransitionProvider(SceneTransitionProvider sceneTransitionProvider) =>
        transitionProvider = sceneTransitionProvider;
}