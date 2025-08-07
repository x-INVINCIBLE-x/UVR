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
    [SerializeField] private float healthSliderSmoothness;
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
    [SerializeField] private float ailmentSliderSmoothness;
    [Space]
    [SerializeField] private GameObject burningUI;
    [SerializeField] private Image burningSlider;
    [Space]
    [SerializeField] private GameObject freezingUI;
    [SerializeField] private Image freezingSlider;
    [Space]
    [SerializeField] private GameObject shockUI;
    [SerializeField] private Image shockSlider;
    [Space]
    [SerializeField] private GameObject drainUI;
    [SerializeField] private Image drainSlider;
    [Space]
    [SerializeField] private GameObject blightUI;
    [SerializeField] private Image blightSlider;
    [Space]
    [SerializeField] private GameObject frenzyUI;
    [SerializeField] private Image frenzySlider;



    private GameObject currentAilmentUI;
    private Image currentAilmentSlider;
    private CanvasGroup ailmentUICanvasGroup;
    
    private Coroutine ailmentRoutine = null;


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

    public void ChangeAilmentUI(bool isActivated, AilmentStatus status)
    {
        if (ailmentRoutine == null)
            ailmentRoutine = StartCoroutine(AilmentLerpRoutine(status));
        
        if (isActivated)
        {
            if (currentAilmentSlider != null)
                currentAilmentSlider.fillAmount = 1;
            // Open some special UI
            if (ailmentRoutine != null)
                StopCoroutine(ailmentRoutine);
            
            ailmentRoutine = null;
            status.AilmentEffectEnded += HandleEffectEnd;
        }
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

    public void SpawnAilmentUI(AilmentType type , bool Activate = true)
    {
        switch (type)
        {
            case AilmentType.Ignis:
                currentAilmentUI = burningUI;
                currentAilmentSlider = burningSlider;
                break;
            case AilmentType.Frost:
                currentAilmentUI = freezingUI;
                currentAilmentSlider = freezingSlider;
                break;
            case AilmentType.Blitz:
                currentAilmentUI = shockUI;
                currentAilmentSlider = shockSlider;
                break;
            case AilmentType.Gaia:
                currentAilmentUI = drainUI;
                currentAilmentSlider = drainSlider;
                break;
            case AilmentType.Radiance:
                currentAilmentUI = blightUI;
                currentAilmentSlider = blightSlider;
                break;
            case AilmentType.Hex:
                currentAilmentUI = frenzyUI;
                currentAilmentSlider = frenzySlider;
                break;          
        }

        ailmentUICanvasGroup = currentAilmentUI.GetComponentInChildren<CanvasGroup>();

        if (ailmentUICanvasGroup != null)
        {
            if (Activate)
            {
                currentAilmentUI.SetActive(true);
                ailmentUICanvasGroup.alpha = 0f;
                StartCoroutine(FadeCanvasGroup(ailmentUICanvasGroup, 0f, 1f, 0.7f));
            }
            else
            {
                StartCoroutine(FadeCanvasGroup(ailmentUICanvasGroup, ailmentUICanvasGroup.alpha, 0f, 0.7f));
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



    private void HandleEffectEnd(AilmentType type)
    {
        Debug.Log($"Ailment Effect Ended: {type}");
        currentAilmentUI.SetActive(false);
    }

    // Handles lerping and Updating of the ailment slider values
    private IEnumerator AilmentLerpRoutine(AilmentStatus status) 
    {
        while (status.Value > 0)
        {   
            Debug.Log($"Ailment Status: {status.Value / status.ailmentLimit}");
            currentAilmentSlider.fillAmount = Mathf.Lerp(currentAilmentSlider.fillAmount,(status.Value / status.ailmentLimit), ailmentSliderSmoothness * Time.deltaTime);
            yield return null;
        }
        currentAilmentSlider.fillAmount = 0;
        ailmentRoutine = null;
    }

    private IEnumerator HealthLerpRoutine(float targetHealthValue)
    {
        while (Mathf.Abs(healthSlider.fillAmount - targetHealthValue) > 0.001f)
        {
            healthSlider.fillAmount = Mathf.Lerp(healthSlider.fillAmount, targetHealthValue, healthSliderSmoothness * Time.deltaTime);
            yield return null;
        }

        healthSlider.fillAmount = targetHealthValue;
    }

    //private IEnumerator AilmentLerpRoutine(float targetValue)
    //{
    //    while (Mathf.Abs(currentAilmentSlider.fillAmount - targetValue) > 0.001f)
    //    {
    //        currentAilmentSlider.fillAmount = Mathf.Lerp(currentAilmentSlider.fillAmount, targetValue, ailmentSliderSmoothness * Time.deltaTime);
    //        yield return null;
    //    }

    //    currentAilmentSlider.fillAmount = targetValue;
    //}


    //private void UpdateHealthSlider(int maxHealth, int currentHealth)
    //{
    //    characterStats.GetHealth();
    //}
}
