using UnityEngine;
using System.Collections;

public class EnemyVFXManager : MonoBehaviour
{
    [Header("VFX")]
    public GameObject MagicCircleAttackVfx;
    public GameObject selfDestructVFX;
    public Transform MagicCircleSpawn;

    [Header("VFX Settings")]
    public Vector3 vfxOffset = new Vector3(0, 0.1f, 0); // Slightly above ground

    [Space]
    public Vector3 minScale = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 midScale = new Vector3(1f, 1f, 1f);
    public Vector3 maxScale = new Vector3(2f, 2f, 2f);

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
    
    
}
