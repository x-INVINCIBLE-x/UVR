using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public enum BuffCategory
{
    Offense,
    Defence,
    Special
}

public class DungeonBuffHandler : MonoBehaviour
{
    [SerializeField] private BuffMaterialInfo[] materialsInfo;

    [SerializeField] private List<DungeonBuffProvider> buffProviders;
    public readonly float Dissolve_Duration = 4f;
    private Dictionary<BuffCategory, BuffMaterialInfo> materialInfoDic = new();

    [Tooltip("After every 3 difficulty increase one new Buff spawner will be created")]
    [SerializeField] private int difficultyScaling = 3;
    [Tooltip("Minimum Buffs to give after level completion")]
    [SerializeField] private int minBuff = 3;
    [Tooltip("Maximum Buffs to give after level completion")]
    [SerializeField] private int buffsCap = 5;

    [SerializeField] private XRSocketInteractor buffHolder;
    [SerializeField] private Vector3 buffOriginOffest; // Centre position from where the holder should start spawning
    [SerializeField] private Vector3 buffHolderOffest; // Space one holder should have from another
    private XRSocketInteractor[] holderInteractors;

    [Header("Buff")]
    public List<BuffGroup> buffGroups = new List<BuffGroup>();
    private Dictionary<int, Dictionary<BuffCategory, List<Buff>>> buffLookup = new();
    private Dictionary<(int, BuffCategory), HashSet<Buff>> shownBuffs = new();

    private int difficultyLevel = 1;

    public event System.Action OnBuffPick;

    private void Awake()
    {
        //foreach (var provider in BuffProviders)
        //{
        //    provider.gameObject.SetActive(false);
        //}
        
        foreach (BuffMaterialInfo info in materialsInfo)
        {
            materialInfoDic[info.Category] = info;
        }
    }

    private void Start()
    {
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

    // Instantiates holder based on current difficulty
    public void Setup(Transform originTransform, int difficultyLevel)
    {
        this.difficultyLevel = difficultyLevel;
        int buffsToProvide = Mathf.Min(buffsCap, minBuff + Mathf.FloorToInt(difficultyLevel / difficultyScaling));
        holderInteractors = new XRSocketInteractor[buffsToProvide];

        Vector3 spawnPosition = originTransform.position + buffOriginOffest;

        int posModifier = 1;
        int j = 0;

        for (int i = 0; i < buffsToProvide; i++)
        {
            GameObject newHolder = Instantiate(buffHolder.gameObject, spawnPosition + (j * posModifier * buffHolderOffest), Quaternion.identity);
            holderInteractors[i] = newHolder.GetComponent<XRSocketInteractor>();
            posModifier *= -1;

            if (i == 0 || i % 2 == 0) 
                j++;
        }

        SetupBuffProviders();
    }

    private void SetupBuffProviders()
    {
        System.Random rng = new System.Random();
        int possibleChoices = Enum.GetValues(typeof(BuffCategory)).Length;
        List<int> numbers = Enumerable.Range(0, possibleChoices).OrderBy(x => rng.Next()).Take(holderInteractors.Length).ToList();

        for (int i = 0; i < holderInteractors.Length; i++)
        {
            int choice = numbers[i];
            var availableBuffs = GetAvailableBuffs((BuffCategory)choice);

            if (availableBuffs == null || availableBuffs.Count == 0) continue; // TODO: Remove

            Buff buffToProvide = availableBuffs[UnityEngine.Random.Range(0, availableBuffs.Count)];

            ProvideBuffs(i, buffToProvide, (BuffCategory)choice);
            BuffView view = holderInteractors[i].GetComponentInChildren<BuffView>();
            if (view != null)
            {
                view.Setup(buffToProvide);
            }

            MarkBuffAsUsed((BuffCategory)choice, buffToProvide);
        }
    }

    public void ProvideBuffs(int index, Buff buffToProvide, BuffCategory buffCategory)
    {
        DungeonBuffProvider newCard = Instantiate(buffToProvide.cardDisplay, holderInteractors[index].transform.position, Quaternion.identity);
        //holderInteractors[index].StartManualInteraction(newCard.GetComponent<IXRSelectInteractable>());
        newCard.Initialize(this, buffToProvide, holderInteractors[index]);
        buffProviders.Add(newCard);
    }

    public List<Buff> GetAvailableBuffs(BuffCategory category)
    {
        if (!buffLookup.TryGetValue(difficultyLevel, out Dictionary<BuffCategory, List<Buff>> categoryDict) ||
            !categoryDict.TryGetValue(category, out List<Buff> allBuffs))
        {
            Debug.LogWarning($"No buffs found for difficulty {difficultyLevel} and category {category}");
            return new List<Buff>();
        }

        var key = (difficultyLevel, category);

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
        var key = (difficultyLevel, category);

        if (!shownBuffs.ContainsKey(key))
            shownBuffs[key] = new HashSet<Buff>();

        shownBuffs[key].Add(selectedBuff);
    }

    public void EnableCardInteraction()
    {
        for (int i = 0; i < buffProviders.Count; i++) 
        { 
            buffProviders[i].SetGrab(true);
        }
    }

    public void DisableCardInteraction(DungeonBuffProvider pickProvider)
    {
        for (int i = 0; i < buffProviders.Count; i++)
        {
            if (pickProvider != buffProviders[i])
            {
                buffProviders[i].SetGrab(false);
            }
        }
    }

    public void BuffPicked()
    {
        foreach (DungeonBuffProvider buffProvider in buffProviders)
        {
            DisableCardInteraction(null);
            buffProvider.Close();
        }
        buffProviders.Clear();

        OnBuffPick?.Invoke();
    }

    //public void AddBuffProvider(DungeonBuffProvider buffProvider) => BuffProviders.Add(buffProvider);
}

[System.Serializable]
public class BuffMaterialInfo
{
    public BuffCategory Category;
    public Material baseMaterial;
    public Material dissolveMaterial;
}
