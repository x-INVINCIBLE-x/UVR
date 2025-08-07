using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [Header("Health UI")]
    [Space]
    [SerializeField] private GameObject HealthUI;
    [SerializeField] private Image healthSlider;
    [SerializeField] private float sliderSmoothness;
    private CanvasGroup healthUICanvasGroup;

    [Header("Exclamation Mark UI")]
    [Space]
    [SerializeField] private GameObject ExclamtionUI;
    private CanvasGroup exclamationUICanvasGroup;

    [Header("Question Mark UI")]
    [Space]
    [SerializeField] private GameObject QuestionUI;
    private CanvasGroup questionUICanvasGroup;

    [Header("Ailment UIs")]
    [Space]
    [SerializeField] private GameObject burningUI;
    [SerializeField] private GameObject freezingUI;
    [SerializeField] private GameObject shockUI;
    [SerializeField] private GameObject drainUI;
    [SerializeField] private GameObject blightUI;
    [SerializeField] private GameObject frenzyUI;

    private GameObject currentAilmentUI;
    private CanvasGroup ailmentUIquestionUICanvasGroup;



    private void Awake()
    {
        healthUICanvasGroup = HealthUI.GetComponentInChildren<CanvasGroup>(); // reference to health ui's canvas group
        exclamationUICanvasGroup = ExclamtionUI.GetComponentInChildren<CanvasGroup>(); // reference to exclamation mark ui's canvas group
        questionUICanvasGroup = QuestionUI.GetComponentInChildren<CanvasGroup>(); // reference to question mark ui's canvas group
    }

    private void Start()
    {
        HealthUI.SetActive(false);
        ExclamtionUI.SetActive(false);
        QuestionUI.SetActive(false);
        burningUI.SetActive(false);
        freezingUI.SetActive(false);
        shockUI.SetActive(false);
        drainUI.SetActive(false);
        blightUI.SetActive(false);
        frenzyUI.SetActive(false);
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

    public void SpawnExclamationUI(bool Activate = true)
    {
        if (exclamationUICanvasGroup != null)
        {
            if (Activate)
            {
                ExclamtionUI.SetActive(true);
                exclamationUICanvasGroup.alpha = 0f;
                StartCoroutine(FadeCanvasGroup(exclamationUICanvasGroup, 0f, 1f, 0.7f));
            }
            else
            {
                StartCoroutine(FadeCanvasGroup(exclamationUICanvasGroup, exclamationUICanvasGroup.alpha, 0f, 0.7f));
            }
        }
    }

    public void SpawnQuestionUI(bool Activate = true)
    {
        if (questionUICanvasGroup != null)
        {
            if (Activate)
            {
                QuestionUI.SetActive(true);
                questionUICanvasGroup.alpha = 0f;
                StartCoroutine(FadeCanvasGroup(questionUICanvasGroup, 0f, 1f, 0.7f));
            }
            else
            {
                StartCoroutine(FadeCanvasGroup(questionUICanvasGroup, questionUICanvasGroup.alpha, 0f, 0.7f));
            }
        }
    }

    public void SpawnAilmentUI(bool Activate = true)
    {
        if(ailmentUIquestionUICanvasGroup != null)
        {

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
