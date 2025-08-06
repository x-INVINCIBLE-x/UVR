using UnityEngine;

public class CurrencyManager : MonoBehaviour ,ISaveable
{   
    public static CurrencyManager Instance {get; private set;}
    [field: SerializeField]public int Gold { get; private set; } = 10000;
    [field: SerializeField]public int Magika { get; private set; } = 10000;

    public event System.Action<int, int> OnCurrencyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameEvents.OnRewardProvided += HandleCurrencyGiven;
    }

    private void OnDisable()
    {
        GameEvents.OnRewardProvided -= HandleCurrencyGiven;
    }

    private void HandleCurrencyGiven(IRewardProvider<GameReward> provider)
    {
        GameReward reward = provider.GetReward();
        AddCurrency(reward.Gold, reward.Magika);
    }

    private void AddCurrency(int gold, int magika)
    {
        Gold += gold;
        Magika += magika;
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

    public class SaveCurrencies
    {
        public int Gold;
        public int Magika;
    }
}
