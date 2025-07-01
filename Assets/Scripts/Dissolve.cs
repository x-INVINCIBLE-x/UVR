using System.Collections;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float dissolveSpeed = 1f;
    [SerializeField] public bool dissolve = false;

    private float _dissolveStrength = 0.25f; // Start fully visible (0 = no dissolve)
    private static readonly int DissolveStrengthID = Shader.PropertyToID("_Dissolve_Strength");

    private Coroutine currentEffect;


    private void Start()
    {
        _dissolveStrength = dissolve ? 0.55f : 0.25f; // Chooses 0.55f if true and 0f if false
        dissolveMaterial.SetFloat(DissolveStrengthID,_dissolveStrength);
    }

    private void Update()
    {
        // Only start effect if it's not already running
       
            if (dissolve && _dissolveStrength < 0.55f)
            {
                currentEffect = StartCoroutine(Dissolving()); // Dissolve
            }
            else if (!dissolve && _dissolveStrength > 0.25f)
            {
                currentEffect = StartCoroutine(Reappear()); // Reappear 
            }
       
        

    }
    //public void StartDissolve()
    //{
    //    StartCoroutine(Dissolving());
    //}

    //public void StartReappear()
    //{
    //    StartCoroutine(Reappear());
    //}

    private IEnumerator Dissolving()
    {
        if (dissolveMaterial == null) yield break;

        while (_dissolveStrength <= 0.55f)
        {
            _dissolveStrength += (dissolveSpeed/1000f) * Time.deltaTime;
            dissolveMaterial.SetFloat(DissolveStrengthID, _dissolveStrength);
            yield return null;
        }

        // Ensure final value is exact
        _dissolveStrength = 0.55f;
        dissolveMaterial.SetFloat(DissolveStrengthID, _dissolveStrength);
    }

    private IEnumerator Reappear()
    {
        if (dissolveMaterial == null) yield break;

        while (_dissolveStrength >= 0.25f)
        {
            _dissolveStrength -= (dissolveSpeed / 1000f) * Time.deltaTime;
            dissolveMaterial.SetFloat(DissolveStrengthID, _dissolveStrength);
            yield return null;
        }

        // Ensure final value is exact
        _dissolveStrength = 0.25f;
        dissolveMaterial.SetFloat(DissolveStrengthID, _dissolveStrength);
        
    }
}