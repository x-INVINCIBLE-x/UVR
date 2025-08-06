using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private GameObject HealthUI;
    private CanvasGroup healthUICanvasGroup;

    [SerializeField] private Image healthSlider;
    [SerializeField] private float sliderSmoothness;

    private void Awake()
    {
        if (HealthUI != null)
        {
            healthUICanvasGroup = HealthUI.GetComponentInChildren<CanvasGroup>(); // reference to health ui's canvas group
        }
    }

    private void Start()
    {
        HealthUI.SetActive(false);
    }

    public void ChangeHealthUI(float healthvalue)
    {
        StartCoroutine(HealthLerpRoutine(healthvalue));
    }
 
    private void Update()
    {
        
    }

   /* private void UpdateHealthSlider(float maxHealth, float currentHealth)
    {   
        float healthRatio = (currentHealth /maxHealth);

        StartCoroutine(HealthLerpRoutine(healthRatio));
    }*/

    public void SpawnHealthUI(bool Activate = true)
    {
        if (healthUICanvasGroup != null)
        {
            if (Activate)
            {
                HealthUI.SetActive(true);
                healthUICanvasGroup.alpha = 0f;
                StartCoroutine(FadeCanvasGroup(healthUICanvasGroup, 0f, 1f, 0.7f));
            }
            else
            {
                StartCoroutine(FadeCanvasGroup(healthUICanvasGroup, healthUICanvasGroup.alpha, 0f, 0.5f));
            }
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        group.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        group.alpha = endAlpha;
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
