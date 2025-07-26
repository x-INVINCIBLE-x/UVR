using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnemyFXHandler : MonoBehaviour
{
    [Header("VFX")]
    public GameObject selfDestructVFX;
    
    [Header("Magic Circle Settings")]
    [Space]
    public GameObject MagicCircleAttackVfx;
    public Transform MagicCircleSpawn;
    public Vector3 vfxOffset = new Vector3(0, 0.1f, 0); // Slightly above ground
    [Space]
    public Vector3 minScale = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 midScale = new Vector3(1f, 1f, 1f);
    public Vector3 maxScale = new Vector3(2f, 2f, 2f);

    [Header("Exclamation Mark UI")]
    [Space]
    public GameObject ExclamationUI;
    public Transform ExclamationSpawn;
    public float ExclamationScale = 250f;
    private GameObject currentExclamationMark;

    [Header("Question Mark UI")]
    [Space]
    public GameObject QuestionMarkUI;
    public Transform QuestionMarkSpawn;
    public float QuestionMarkScale = 250f;
    private GameObject currentQuestionMark;

    private GameObject spawnedVFX;
    

    public void SpawnMagicCircleVFX(float chargeTime)
    {
        if (MagicCircleAttackVfx == null)
        {
            //Debug.LogWarning("enemyIntiateAttackVfx is not assigned!");
            return;
        }

        //Debug.Log("Spawning Magic Circle VFX");
        spawnedVFX = Instantiate(MagicCircleAttackVfx, MagicCircleSpawn);

        // Start coroutine to animate scaling
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
            if (spawnedVFX != null)
                spawnedVFX.transform.localScale = scale;

            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure it reaches midScale
        if (spawnedVFX != null)
            spawnedVFX.transform.localScale = midScale;

        // Phase 2: Exponential (ease-out-like) Lerp from mid to max
        timer = 0f;
        while (timer < halfTime)
        {
            float t = timer / halfTime;
            float expT = t * t; // Exponential progression
            Vector3 scale = Vector3.Lerp(midScale, maxScale, expT);
            if (spawnedVFX != null)
                spawnedVFX.transform.localScale = scale;

            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure it reaches maxScale at the end
        if (spawnedVFX != null)
            spawnedVFX.transform.localScale = maxScale;
    }

    public void DestroyMagicCircleVFX()
    {
        if (spawnedVFX != null)
        {
            Destroy(spawnedVFX);
            spawnedVFX = null;
        }
    }

    public void SelfDestructingVFX(float lifetime)
    {
        if (selfDestructVFX == null) return;

        if (spawnedVFX != null) return; // Prevents duplicates

        spawnedVFX = Instantiate(selfDestructVFX,this.transform);

        Destroy(spawnedVFX,lifetime);
    }

    public void ActivateMagicCircle()
    {
        if (spawnedVFX != null) return; // Prevents duplicates
        spawnedVFX = Instantiate(MagicCircleAttackVfx, MagicCircleSpawn.position + vfxOffset, Quaternion.identity);

    }

    public void DestroyMagicCircle()
    {
        if (spawnedVFX == null) return; // Prevents duplicates
        Destroy(spawnedVFX);
    }

    public void SpawnExclamationMark(bool Activate = true)
    {
       
        if (Activate)
        {
            if (currentExclamationMark != null)
                return; 

            // Spawn new exclamation mark
            currentExclamationMark = Instantiate(ExclamationUI, ExclamationSpawn.position, Quaternion.identity,ExclamationSpawn);
            currentExclamationMark.transform.localScale = new Vector3(1f * ExclamationScale, 1f * ExclamationScale, 1f * ExclamationScale);
            CanvasGroup exclamationCanvasGroup = currentExclamationMark.GetComponentInChildren<CanvasGroup>();

            exclamationCanvasGroup.alpha = 0.0f;
            exclamationCanvasGroup.DOFade(1, 0.7f); // Fade in
        }

       
        else
        {
            if (currentExclamationMark == null)
            {
               
                return;
            }

            CanvasGroup exclamationCanvasGroup = currentExclamationMark.GetComponentInChildren<CanvasGroup>();
            if (exclamationCanvasGroup != null)
            {
               
                exclamationCanvasGroup.DOFade(0, 0.7f).OnComplete(() =>
                {
                    Destroy(currentExclamationMark); 
                    currentExclamationMark = null;   
                });
            }
        }
    }

    public void SpawnQuestionMark(bool Activate = true)
    {

        if (Activate)
        {
            if (currentQuestionMark != null)
                return;

            // Spawn new Question mark
            currentQuestionMark = Instantiate(QuestionMarkUI,QuestionMarkSpawn.position, Quaternion.identity, QuestionMarkSpawn);
            currentQuestionMark.transform.localScale = new Vector3(1f * QuestionMarkScale, 1f * QuestionMarkScale, 1f * QuestionMarkScale);
            CanvasGroup questionCanvasGroup = currentQuestionMark.GetComponentInChildren<CanvasGroup>();

            questionCanvasGroup.alpha = 0.0f;
            questionCanvasGroup.DOFade(1, 0.7f); // Fade in
        }


        else
        {
            if (currentQuestionMark == null) return;
            
            CanvasGroup questionCanvasGroup = currentQuestionMark.GetComponentInChildren<CanvasGroup>();
            if (questionCanvasGroup != null)
            {
                questionCanvasGroup.DOFade(0, 0.7f).OnComplete(() =>
                {
                    Destroy(currentQuestionMark);
                    currentQuestionMark = null;
                });
            }
        }
    }

}
