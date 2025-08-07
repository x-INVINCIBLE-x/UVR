using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyFXHandler : MonoBehaviour
{
    [Header("Character UI")]
    [SerializeField] private CharacterUI characterUI;

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

    

    [Header("Question Mark UI")]
    [Space]
    public GameObject QuestionMarkUI;
    public Transform QuestionMarkSpawn;
    public float QuestionMarkScale = 250f;
    private GameObject currentQuestionMark;

    private GameObject spawnedVFX;

    private void Start()
    {
    
    }
   
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
    public void UpdateHealthValue(float value)
    {
        characterUI.ChangeHealthUI(value);
    }

    public void UpdateAilmentValue(bool Activated, AilmentStatus ailmentStatus)
    {
        characterUI.ChangeAilmentUI(Activated , ailmentStatus);
    }

}
