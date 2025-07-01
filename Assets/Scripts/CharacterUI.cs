using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private Image healthSlider;
    [SerializeField] private float tempMaxHealth;
    [SerializeField] private float tempCurrentHealth;
    [SerializeField] private float sliderSmoothness;

    
    
    private void Start()
    {
        characterStats.OnDamageTaken += UpdateHealthSlider;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tempCurrentHealth -= 50f;
            UpdateHealthSlider(tempMaxHealth, tempCurrentHealth);
        }
    }

    private void UpdateHealthSlider(float maxHealth, float currentHealth)
    {   
        float healthRatio = (currentHealth /maxHealth);

        StartCoroutine(HealthLerpRoutine(healthRatio));
    }

    private IEnumerator HealthLerpRoutine(float targetHealthValue)
    {
        while (Mathf.Abs(healthSlider.fillAmount - targetHealthValue) > 0.001f)
        {
            healthSlider.fillAmount = Mathf.Lerp(healthSlider.fillAmount, targetHealthValue, sliderSmoothness * Time.deltaTime);
            yield return null;
        }

        healthSlider.fillAmount = targetHealthValue;
    }

    //private void UpdateHealthSlider(int maxHealth, int currentHealth)
    //{
    //    characterStats.GetHealth();
    //}
}
