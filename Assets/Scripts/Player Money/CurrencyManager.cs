using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour ,ISaveable
{   
    public static CurrencyManager Instance {get; private set;}
    [field: SerializeField]public int Gold { get; private set; } = 10000;
    [field: SerializeField]public int Magika { get; private set; } = 10000;

    [SerializeField] private float baseGoldMultiplier = 1f;
    [SerializeField] private float baseMagikaMultiplier = 1f;

    private List<float> goldMultipliers = new List<float>();
    private List<float> magikaMultipliers = new List<float>();

    public float CurrentGoldMultiplier { get; private set; } = 1f;
    public float CurrentMagikaMultiplier { get; private set; } = 1f;

    public event System.Action<int, int> OnCurrencyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        GameEvents.OnRewardProvided += HandleCurrencyGiven;
    }

    private void OnDestroy()
    {
        GameEvents.OnRewardProvided -= HandleCurrencyGiven;
    }
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void AddCurrencyMultiplier(float goldMulti, float magikaMulti)
    {
        if (goldMulti != 0f) goldMultipliers.Add(goldMulti);
        if (magikaMulti != 0f) magikaMultipliers.Add(magikaMulti);
        UpdateMultipliers();
    }

    public void RemoveCurrencyMultiplier(float goldMulti, float magikaMulti)
    {
        goldMultipliers.Remove(goldMulti);
        magikaMultipliers.Remove(magikaMulti);
        UpdateMultipliers();
    }

    private void UpdateMultipliers()
    {
        float goldSum = 0f;
        foreach (var g in goldMultipliers)
            goldSum += g;

        float magikaSum = 0f;
        foreach (var m in magikaMultipliers)
            magikaSum += m;

        CurrentGoldMultiplier = baseGoldMultiplier + goldSum;
        CurrentMagikaMultiplier = baseMagikaMultiplier + magikaSum;
    }

    private void HandleCurrencyGiven(IRewardProvider<GameReward> provider)
    {
        GameReward reward = provider.GetReward();
        AddCurrency(reward.Gold, reward.Magika);
    }

    private void AddCurrency(int gold, int magika)
    {
        int finalGold = Mathf.RoundToInt(gold * CurrentGoldMultiplier);
        int finalMagika = Mathf.RoundToInt(magika * CurrentMagikaMultiplier);

        Gold += finalGold;
        Magika += finalMagika;

        OnCurrencyChanged?.Invoke(Gold, Magika);

        // Trigger UI update or audio feedback if needed
    }

    public bool SpendGold(int amount)
    {
        if(Gold >= amount)
        {
            Gold -= amount;
            OnCurrencyChanged?.Invoke(Gold, Magika);
            return true;
        }

        return false;
    }

    public bool SpendMagika(int amount)
    {
        if (Magika >= amount)
        {
            Magika -= amount;
            OnCurrencyChanged?.Invoke(Gold, Magika);
            return true;
        }
        return false;
    }

    public int GetGold() => Gold;
    public int GetMagika() => Magika;

    public object CaptureState()
    {
        SaveCurrencies saveCurrencies = new();
        saveCurrencies.Gold = Gold;
        saveCurrencies.Magika = Magika;
        return saveCurrencies;
    }

    public void RestoreState(object state)
    {
        SaveCurrencies saveCurrencies = (SaveCurrencies)state;
        Gold = saveCurrencies.Gold;
        Magika = saveCurrencies.Magika;
    }

    [System.Serializable]
    public class SaveCurrencies
    {
        public int Gold;
        public int Magika;
    }
}
