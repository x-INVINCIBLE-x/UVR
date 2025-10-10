using TMPro;
using UnityEngine;

public class CurrencyRewardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI magikaText;
    [SerializeField] private GameObject goldUI;
    [SerializeField] private GameObject magikaUI;

    private void OnEnable()
    {
        CurrencyManager currencyManager = CurrencyManager.Instance;

        //if (currencyManager != null)
        //{
        //    currencyManager.OnCurrencyChanged += HandleCurrencyChange;
        //}
    }

    public void UpdateUI(int gold, int magika)
    { 
        goldUI.SetActive(gold > 0);
        magikaUI.SetActive(magika > 0);

        goldText.text = gold.ToString();
        magikaText.text = magika.ToString();
    }

    private void OnDisable()
    {
        //if (CurrencyManager.Instance != null)
        //{
        //    CurrencyManager.Instance.OnCurrencyChanged -= HandleCurrencyChange;
        //}
    }
}
