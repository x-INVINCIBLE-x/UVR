using System.Collections;
using UnityEngine;
public class MeshDissolver : MonoBehaviour
{
    [Header("References")]
    public MeshRenderer[] Meshes;
    public Material DissolveMaterial;
    public bool Dissolve;
    private bool isDissolving;

    [SerializeField] private Material[] defaultMaterials;

    [SerializeField] private float dissolveRate = 0.0125f;
    [SerializeField] private float RefreshRate = 0.025f;

    [SerializeField] private float impactDissolveRate = 0.0125f;
    [SerializeField] private float impactDissolveThreshold = 0.5f;

    [SerializeField] private float currentDissolve;

    private Coroutine dissolveCoroutine;
    private Coroutine completeDissolveCoroutine;

    private static readonly int DissolveAmountID = Shader.PropertyToID("_Dissolve_Amount");

    private void Awake()
    {
        if (DissolveMaterial == null)
        {
            Debug.Log("Dissolve Material not Assigned");
        }

        Meshes = GetComponentsInChildren<MeshRenderer>();
        defaultMaterials = new Material[Meshes.Length];


        for (int i = 0; i < Meshes.Length; i++)
        {
            defaultMaterials[i] = Meshes[i].material;
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ResetDissolver();
        //}

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    ImpactPartialDissolve();
        //}

        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    ImpactPartialRedissolve();
        //}


        // to remove this update statements(important)
        if (Dissolve)
        {
            //StartDissolver();


        }
        else
        {
            //ResetDissolver();

        }
    }


    /*    public void ActivateDissolver(bool dissolveStatus)
        {
            Dissolve = dissolveStatus;

            if (dissolveStatus)
            {
                ImpactPartialDissolve();
            }
            else
            {
                ImpactPartialRedissolve();
            }
        }*/
    public void StartDissolver()
    {
        if (isDissolving && dissolveCoroutine != null)
            StopCoroutine(dissolveCoroutine);

        isDissolving = true;
        foreach (MeshRenderer renderer in Meshes)
        {
            renderer.material = DissolveMaterial;
        }

        completeDissolveCoroutine = StartCoroutine(DissolveMesh());
    }

    public void StartImpactDissolve(float duration)
    {
        StartCoroutine(ImpactDissolveRoutine(duration));
    }

    private IEnumerator ImpactDissolveRoutine(float dissolveDuration)
    {
        ImpactPartialDissolve();
        yield return dissolveCoroutine;
        yield return new WaitForSeconds(dissolveDuration);
        if (completeDissolveCoroutine == null)
            ImpactPartialRedissolve();
    }

    public void ImpactPartialDissolve()
    {
        if (isDissolving)
            StopCoroutine(dissolveCoroutine);

        isDissolving = true;
        foreach (MeshRenderer renderer in Meshes)
        {
            renderer.material = DissolveMaterial;
        }
        dissolveCoroutine = StartCoroutine(DissolveMesh(impactDissolveThreshold, false));

    }

    public void ImpactPartialRedissolve()
    {
        if (isDissolving)
            StopCoroutine(dissolveCoroutine);

        isDissolving = true;
        foreach (MeshRenderer renderer in Meshes)
        {
            renderer.material = DissolveMaterial;
        }
        dissolveCoroutine = StartCoroutine(RedissolveMesh());
    }


    public void ResetDissolver()
    {
        StopAllCoroutines();
        isDissolving = false;

        // Restore original material to  Meshrenderers
        for (int i = 0; i < Meshes.Length; i++)
        {
            Meshes[i].material = defaultMaterials[i];
        }

    }

    // Important disclaimer if the parameter dissolve is true ,then normal disolve
    // if dissolve is false then we do a specified threshold dissolve
    private IEnumerator DissolveMesh(float dissolveThreshold = 1f, bool dissolve = true)
    {
        if (Meshes.Length > 0)
        {
            currentDissolve = 0;

            float currentDissolveRate = dissolve ? dissolveRate : impactDissolveRate;


            while (currentDissolve < dissolveThreshold)
            {
                currentDissolve += currentDissolveRate;

                currentDissolve = Mathf.Clamp01(currentDissolve);

                for (int i = 0; i < Meshes.Length; i++)
                {
                    Meshes[i].material.SetFloat(DissolveAmountID, currentDissolve);
                }

                yield return new WaitForSeconds(RefreshRate);
            }

            // reset isDissolving for next calls
            isDissolving = false;
            dissolveCoroutine = null;
            completeDissolveCoroutine = null;
        }
    }

    // Redissolves the Mesh renderers to 0 dissolve rate
    private IEnumerator RedissolveMesh(float dissolveThreshold = 0.0f)
    {
        if (Meshes.Length > 0)
        {


            while (currentDissolve > dissolveThreshold)
            {
                currentDissolve -= impactDissolveRate;

                currentDissolve = Mathf.Clamp01(currentDissolve);

                for (int i = 0; i < Meshes.Length; i++)
                {
                    Meshes[i].material.SetFloat(DissolveAmountID, currentDissolve);
                }

                yield return new WaitForSeconds(RefreshRate);
            }

            // reset isDissolving for next calls
            isDissolving = false;
        }

    }
    private void OnDisable()
    {
        ResetDissolver();
        StopAllCoroutines();
    }

}

