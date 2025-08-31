using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyFXHandler : MonoBehaviour
{
    [Header("References")]
    [Space]
    [SerializeField] private CharacterUI characterUI; // Used for managing the character UI (Ailment type UI, Exclamation UI, QuestionMark UI , etc...)
    [SerializeField] private CharacterStatusVfx characterStatusVfx; // Used for managing the character VFX (Ailment types VFXs , etc...)

    [Header("VFX")]
    public GameObject selfDestructVFX;
    private GameObject currentSelfDestructVFX;
    
    [Header("Magic Circle Settings")]
    [Space]
    [SerializeField] private GameObject MagicCircle;
    [Space]
    public Vector3 minScale = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 midScale = new Vector3(1f, 1f, 1f);
    public Vector3 maxScale = new Vector3(2f, 2f, 2f);


    private void Awake()
    {   
        if(MagicCircle != null)
            MagicCircle.SetActive(false);
    }
    private void Start()
    {
    
    }

    public void SpawnMagicCircleVFX(float chargeTime)
    {
        if (MagicCircle == null)
        {
            return;
        }

        // Reset scale and position before enabling
        MagicCircle.transform.localScale = minScale;
        MagicCircle.SetActive(true);
        StartCoroutine(AnimateMagicCircleScale(chargeTime));
    }

    private IEnumerator AnimateMagicCircleScale(float totalTime)
    {
        float halfTime = totalTime / 2f;
        float timer = 0f;

        // Phase 1: Linear Lerp from min to mid
        while (timer < halfTime)
        {
            float t = timer / halfTime;
            Vector3 scale = Vector3.Lerp(minScale, midScale, t);
            if (MagicCircle != null)
                MagicCircle.transform.localScale = scale;

            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure it reaches midScale
        if (MagicCircle != null)
            MagicCircle.transform.localScale = midScale;

        // Phase 2: Exponential (ease-out-like) Lerp from mid to max
        timer = 0f;
        while (timer < halfTime)
        {
            float t = timer / halfTime;
            float expT = t * t; // Exponential progression
            Vector3 scale = Vector3.Lerp(midScale, maxScale, expT);
            if (MagicCircle != null)
                MagicCircle.transform.localScale = scale;

            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure it reaches maxScale at the end
        if (MagicCircle != null)
            MagicCircle.transform.localScale = maxScale;
    }

    public void DestroyMagicCircleVFX()
    {
        if (MagicCircle != null)
        {
            MagicCircle.SetActive(false);
        }
    }

    public void SelfDestructingVFX(float lifetime)
    {
        if (selfDestructVFX == null) return;

        if (currentSelfDestructVFX != null) return; // Prevents duplicates

        currentSelfDestructVFX = Instantiate(selfDestructVFX,this.transform);

        Destroy(currentSelfDestructVFX,lifetime);
    }
    public void SpawnExclamationMark(bool Activate = true)
    {
        characterUI.SpawnExclamationUI(Activate);
    }
    public void SpawnQuestionMark(bool Activate = true)
    {
        characterUI.SpawnQuestionUI(Activate);
    }

    public void SpawnHealthUI(bool Activate = true)
    {   
        characterUI.SpawnHealthUI(Activate);
    }
    public void SpawnAilmentUI(AilmentType type, bool Activate = true)
    {
        characterUI.SpawnAilmentUI(type, Activate);
    }
    public void SpawnAilmentVFX(AilmentType type , bool Activate = true)
    {
        characterStatusVfx.SpawnStatusVFX(type, Activate);
    }
    public void UpdateHealthValue(float value)
    {
        characterUI.ChangeHealthUI(value);
    }
    public void UpdateAilmentValue(bool Activated, AilmentStatus ailmentStatus)
    {
        characterUI.ChangeAilmentUI(Activated , ailmentStatus);
    }


}
