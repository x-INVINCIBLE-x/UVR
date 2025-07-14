using UnityEngine;

public class CurrencyManager : MonoBehaviour ,ISaveable
{   
    public static CurrencyManager Instance {get; private set;}
    [field: SerializeField]public int Gold { get; private set; } = 10000;
    [field: SerializeField]public int Magika { get; private set; } = 10000;

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
        GameEvents.OnCurrencyGiven += HandleCurrencyGiven;
    }

    private void OnDisable()
    {
        GameEvents.OnCurrencyGiven -= HandleCurrencyGiven;
    }

    private void HandleCurrencyGiven(IRewardProvider provider)
    {
        (int _gold, int _magika) = provider.GetCurrencyReward();
        AddCurrency(_gold, _magika);
    }

    private void AddCurrency(int gold, int magika)
    {
        Gold += gold;
        Magika += magika;
        // Trigger UI update or audio feedback if needed
    }

    public bool SpendGold(int amount)
    {
        if(Gold >= amount)
        {
            Gold -= amount;
            return true;
        }
        return false;
    }

    public bool SpendMagika(int amount)
    {
        if (Magika >= amount)
        {
            Magika -= amount;
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
