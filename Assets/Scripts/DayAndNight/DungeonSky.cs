using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DungeonSky : MonoBehaviour
{
    [SerializeField] private Texture2D skybox_1;
    [SerializeField] private Texture2D skybox_2;

    [SerializeField] private Gradient gradient;


    [SerializeField] private Light globalLight;
    [SerializeField] private float switchTime = 3f;
    [SerializeField] private bool hasEntered;

    private void Start()
    {
        RenderSettings.skybox.SetTexture("_Texture1", skybox_1);
        RenderSettings.skybox.SetTexture("_Texture2", skybox_2);
        RenderSettings.skybox.SetFloat("_Blend", 0);
    }

    private IEnumerator LerpSkybox(Texture2D a, Texture2D b, float time)
    {
        RenderSettings.skybox.SetTexture("_Texture1", a);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", 0);
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", i / time);
            yield return null;
        }
        RenderSettings.skybox.SetTexture("_Texture1", b);
    }

    private IEnumerator LerpLight(Gradient lightGradient, float time)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            globalLight.color = lightGradient.Evaluate(i / time);
            RenderSettings.fogColor = globalLight.color;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasEntered)
        {
            hasEntered = true;
            StartCoroutine(LerpSkybox(skybox_1, skybox_2, 2f));
            StartCoroutine(LerpLight(gradient, 2f));
            Debug.Log("Trigger Activated");
        }
        
    }

}
