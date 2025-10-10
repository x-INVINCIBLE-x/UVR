using System;
using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI magikaText;

    private void OnEnable()
    {
        CurrencyManager currencyManager = CurrencyManager.Instance;

        if (currencyManager != null)
        {
            currencyManager.OnCurrencyChanged += HandleCurrencyChange;

            goldText.text = currencyManager.Gold.ToString();
            magikaText.text = currencyManager.Magika.ToString();
        }
    }

    private void HandleCurrencyChange(int gold, int magika)
    {
        goldText.text = gold.ToString();
        magikaText.text = magika.ToString();
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged -= HandleCurrencyChange;
        }
    }
}
