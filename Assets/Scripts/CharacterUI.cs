using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [Header("Health UI")]
    [Space]
    [SerializeField] private GameObject HealthUI;
    [SerializeField] private Image healthSlider;
    [SerializeField, Min(1)] private float healthSliderSmoothness;
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
    [Space, Min(1)]
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



    private Dictionary<AilmentType, GameObject> ailmentUIs;
    private Dictionary<AilmentType, Image> ailmentSliders;
    private Dictionary<AilmentType, CanvasGroup> ailmentCanvasGroups;

    private Dictionary<AilmentStatus, Coroutine> ailmentRoutines = new Dictionary<AilmentStatus, Coroutine>();
    private Coroutine ailmentRoutine = null;
    private Coroutine healthRoutine = null;
    private float targetHealthValue = 1f;


    private void Awake()
    {
        healthUICanvasGroup = HealthUI.GetComponentInChildren<CanvasGroup>();
        exclamationUICanvasGroup = ExclamtionUI.GetComponentInChildren<CanvasGroup>();
        questionUICanvasGroup = QuestionUI.GetComponentInChildren<CanvasGroup>();


        ailmentUIs = new Dictionary<AilmentType, GameObject>
    {
        { AilmentType.Ignis, burningUI },
        { AilmentType.Frost, freezingUI },
        { AilmentType.Blitz, shockUI },
        { AilmentType.Gaia, drainUI },
        { AilmentType.Radiance, blightUI },
        { AilmentType.Hex, frenzyUI },
    };

        ailmentSliders = new Dictionary<AilmentType, Image>
    {
        { AilmentType.Ignis, burningSlider },
        { AilmentType.Frost, freezingSlider },
        { AilmentType.Blitz, shockSlider },
        { AilmentType.Gaia, drainSlider },
        { AilmentType.Radiance, blightSlider },
        { AilmentType.Hex, frenzySlider },
    };

        ailmentCanvasGroups = new Dictionary<AilmentType, CanvasGroup>();

        foreach (var kvp in ailmentUIs)
        {
            if (kvp.Value != null)
            {
                var canvasGroup = kvp.Value.GetComponentInChildren<CanvasGroup>();
                ailmentCanvasGroups[kvp.Key] = canvasGroup;
            }
        }
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
        targetHealthValue = healthvalue;
        if (healthRoutine == null)
            StartCoroutine(HealthLerpRoutine());
    }

    public void ChangeAilmentUI(bool isActivated, AilmentStatus status)
    {
        AilmentType type = status.Type;

        if (!ailmentRoutines.ContainsKey(status) || ailmentRoutines[status] == null)
        {
            Coroutine routine = StartCoroutine(AilmentLerpRoutine(status));
            ailmentRoutines[status] = routine;
        }

        if (isActivated)
        {
            if (ailmentSliders.TryGetValue(type, out var slider))
                slider.fillAmount = 1;

            if (ailmentRoutines[status] != null)
            {
                StopCoroutine(ailmentRoutines[status]);
                ailmentRoutines[status] = null;
            }

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

    public void SpawnAilmentUI(AilmentType type, bool Activate = true)
    {
        if (!ailmentUIs.ContainsKey(type)) return;

        GameObject ui = ailmentUIs[type];
        CanvasGroup group = ailmentCanvasGroups[type];

        if (group != null)
        {
            if (Activate)
            {
                ui.SetActive(true);
                group.alpha = 0f;
                StartCoroutine(FadeCanvasGroup(group, 0f, 1f, 0.7f));
            }
            else
            {
                StartCoroutine(FadeCanvasGroup(group, group.alpha, 0f, 0.7f));
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
        if (ailmentUIs.TryGetValue(type, out var ui))
            ui.SetActive(false);
    }


    private IEnumerator AilmentLerpRoutine(AilmentStatus status)
    {
        AilmentType type = status.Type;

        if (!ailmentSliders.ContainsKey(type) || !ailmentUIs.ContainsKey(type))
            yield break;

        Image slider = ailmentSliders[type];
        GameObject ui = ailmentUIs[type];

        float normalizedValue = (float)Math.Round((status.Value / status.ailmentLimit), 2);

        while (status.Value > 0)
        {
            normalizedValue = (float)Math.Round((status.Value / status.ailmentLimit), 2);
            slider.fillAmount = Mathf.Lerp(slider.fillAmount, normalizedValue, ailmentSliderSmoothness * Time.deltaTime);
            yield return null;
        }

        slider.fillAmount = 0;
        ui.SetActive(false);
        ailmentRoutines[status] = null;
    }


    private IEnumerator HealthLerpRoutine()
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
