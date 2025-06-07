using UnityEngine;

public class MoneyManager : MonoBehaviour ,ISaveable
{   

    public static MoneyManager Instance {get; private set;}
    [field: SerializeField]public int Gold { get; private set; } = 10000;
    [field: SerializeField]public int magika { get; private set; } = 10000;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool SpendMoney(int amount)
    {
        if(Gold >= amount)
        {
            Gold -= amount;
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        Gold += amount;
    }

    public object CaptureState()
    {
        SaveCurrencies saveCurrencies = new SaveCurrencies();
        saveCurrencies.Gold = Gold;
        saveCurrencies.Magika = magika;
        return saveCurrencies;
    }

    public void RestoreState(object state)
    {
        SaveCurrencies saveCurrencies = (SaveCurrencies)state;
        Gold = saveCurrencies.Gold;
        magika = saveCurrencies.Magika;
    }

    public class SaveCurrencies
    {
        public int Gold;
        public int Magika;
    }
}
