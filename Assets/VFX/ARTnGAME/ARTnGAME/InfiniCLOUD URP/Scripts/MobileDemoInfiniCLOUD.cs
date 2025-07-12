using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileDemoInfiniCLOUD : MonoBehaviour
{
    //Camera views
    public List<Transform> cameraPositions = new List<Transform>();

    //Extra cloud layers
    public GameObject cloudLayerMAIN;
    public GameObject cloudLayer0;
    public GameObject cloudLayerA;
    public GameObject cloudLayerB;

    //VORTEX
    public GameObject vortexCloudLayer;

    //SUN
    public Light sunLight;

    //Particle volume lit clouds
    public GameObject ParticleClouds;
    //public ParticleSystem particles;
    public ParticleSystemRenderer particlesRenderer;

    //Upper cloud layer
    public GameObject upperCloudLayer;
    public Material upperCloudsMaterial;
    public bool smoothFadeOut = false;

    public GameObject simpleCloudLayer;
    public Material simpleCloudsMaterial;

    public float toggleParticleRenderOrderY = 510;

    //Shadow layer
    public GameObject shadowsLayer;

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.resolutionScalingFixedDPIFactor = 0.75f;
        upperCloudsMaterial.SetFloat("_Transparency", minTransp);
        simpleCloudsMaterial.SetFloat("_CloudCover", minTranspA);
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null && Camera.main.transform.position.y < toggleParticleRenderOrderY)
        {
            if (particlesRenderer.sortingOrder != -1)
            {
                particlesRenderer.sortingOrder = -1;
            }

            if (!smoothFadeOut)
            {
                if (upperCloudLayer.activeInHierarchy)
                {
                    upperCloudLayer.SetActive(false);
                }
            }
            else
            {
                //upperCloudsMaterial.SetFloat("_Coverage", Mathf.Lerp(upperCloudsMaterial.GetFloat("_Coverage"), 0, Time.deltaTime * 2));
                upperCloudsMaterial.SetFloat("_Transparency", Mathf.Lerp(upperCloudsMaterial.GetFloat("_Transparency"), maxTransp + transparencyOffset, Time.deltaTime * 3));
                simpleCloudsMaterial.SetFloat("_CloudCover", Mathf.Lerp(simpleCloudsMaterial.GetFloat("_CloudCover"), maxTranspA + transparencyOffset, Time.deltaTime * 3));
            }
        }
        else
        {
            if (particlesRenderer.sortingOrder != 1)
            {
                particlesRenderer.sortingOrder = 1;
            }
            if (!smoothFadeOut)
            {
                if (!upperCloudLayer.activeInHierarchy)
                {
                    upperCloudLayer.SetActive(true);
                }
            }
            else
            {
                // upperCloudsMaterial.SetFloat("_Coverage", Mathf.Lerp(upperCloudsMaterial.GetFloat("_Coverage"), 1.5f, Time.deltaTime * 2));
                upperCloudsMaterial.SetFloat("_Transparency", Mathf.Lerp(upperCloudsMaterial.GetFloat("_Transparency"), minTransp + transparencyOffset, Time.deltaTime * 3));
                simpleCloudsMaterial.SetFloat("_CloudCover", Mathf.Lerp(simpleCloudsMaterial.GetFloat("_CloudCover"), minTranspA + transparencyOffset, Time.deltaTime * 3));
            }
        }
    }
    public float minTransp = -26;
    public float maxTransp = -27.52f;
    public float minTranspA = -26;
    public float maxTranspA = -27.52f;
    public float transparencyOffset = 0;
    int currentCameraPos = 0;
    private void OnGUI()
    {
        int rightSide = 200;

        //float transp = upperCloudsMaterial.GetFloat("_Transparency");//, -26);
        GUI.Label(new Rect(rightSide + 150 * 0, 100, 150, 30), "Transparency Offset");
        transparencyOffset = GUI.HorizontalSlider(new Rect(rightSide + 150 * 0, 125, 150, 40), transparencyOffset, -2, 2);

        if (GUI.Button(new Rect(rightSide + 150 * 0, 190, 150, 40), "Cycle Camera Positions"))
        {
            if (currentCameraPos >= cameraPositions.Count)
            {
                currentCameraPos = 0;
                Camera.main.transform.position = cameraPositions[currentCameraPos].position;
                Camera.main.transform.rotation = cameraPositions[currentCameraPos].rotation;
            }
            else
            {
                currentCameraPos += 1;
                if (currentCameraPos < cameraPositions.Count)
                {
                    Camera.main.transform.position = cameraPositions[currentCameraPos].position;
                    Camera.main.transform.rotation = cameraPositions[currentCameraPos].rotation;
                }
            }
        }

        string buttonText0 = "on";
        if (!cloudLayer0.activeInHierarchy)
        {
            buttonText0 = "off";
        }
        if (GUI.Button(new Rect(rightSide - 150 * 1, 10+31, 110, 21), "Layer 0 " + buttonText0))
        {
        }
        if (GUI.Button(new Rect(rightSide - 150 * 1, 10, 150, 30), "Toggle Cloud Layer 0"))
        {
            if (cloudLayer0.activeInHierarchy)
            {
                cloudLayer0.SetActive(false);
            }
            else
            {
                cloudLayer0.SetActive(true);
            }
        }

        string buttonText1 = "on";
        if (!cloudLayerA.activeInHierarchy)
        {
            buttonText1 = "off";
        }
        if (GUI.Button(new Rect(rightSide + 150 * 0, 10+31, 110, 21), "Layer A " + buttonText1))
        {
        }
        if (GUI.Button(new Rect(rightSide + 150 * 0, 10, 150, 30), "Toggle Cloud Layer A"))
        {
            if (cloudLayerA.activeInHierarchy)
            {
                cloudLayerA.SetActive(false);
            }
            else
            {
                cloudLayerA.SetActive(true);
            }
        }

        string buttonText2 = "on";
        if (!cloudLayerB.activeInHierarchy)
        {
            buttonText2 = "off";
        }
        if (GUI.Button(new Rect(rightSide + 150 * 1, 10 + 31, 110, 21), "Layer B " + buttonText2))
        {
        }
        if (GUI.Button(new Rect(rightSide + 150 * 1, 10, 150, 30), "Toggle Cloud Layer B"))
        {
            if (cloudLayerB.activeInHierarchy)
            {
                cloudLayerB.SetActive(false);
            }
            else
            {
                cloudLayerB.SetActive(true);
            }
        }

        string buttonText3 = "on";
        if (!upperCloudLayer.activeInHierarchy)
        {
            buttonText3 = "off";
        }
        if (GUI.Button(new Rect(rightSide + 150 * 2, 10 + 31, 110, 21), "Upper Layer " + buttonText3))
        {
        }
        if (GUI.Button(new Rect(rightSide + 150 * 2, 10, 150, 30), "Toggle Upper Clouds"))
        {
            if (upperCloudLayer.activeInHierarchy)
            {
                upperCloudLayer.SetActive(false);
            }
            else
            {
                upperCloudLayer.SetActive(true);
            }
        }    
        if (GUI.Button(new Rect(rightSide + 150 * 3, 10, 150,30), "Toggle Particle Clouds"))
        {
            if (ParticleClouds.activeInHierarchy)
            {
                ParticleClouds.SetActive(false);
            }
            else
            {
                ParticleClouds.SetActive(true);
            }
        }
        if (GUI.Button(new Rect(rightSide + 150 * 4, 10, 150, 30), "Toggle Cloud Shadows"))
        {
            if (shadowsLayer.activeInHierarchy)
            {
                shadowsLayer.SetActive(false);
            }
            else
            {
                shadowsLayer.SetActive(true);
            }
        }
        if (GUI.Button(new Rect(rightSide + 150 * 5, 10, 150, 30), "Toggle Cloud Vortex"))
        {
            if (vortexCloudLayer.activeInHierarchy)
            {
                vortexCloudLayer.SetActive(false);

                //others
                shadowsLayer.SetActive(true);
                ParticleClouds.SetActive(true);
                upperCloudLayer.SetActive(true);
                cloudLayerA.SetActive(false);
                cloudLayerB.SetActive(false);
                cloudLayerMAIN.SetActive(true);
                Camera.main.transform.position = cameraPositions[0].position;
                Camera.main.transform.rotation = cameraPositions[0].rotation;
            }
            else
            {
                vortexCloudLayer.SetActive(true);
                Camera.main.transform.position = cameraPositions[cameraPositions.Count - 1].position;
                Camera.main.transform.rotation = cameraPositions[cameraPositions.Count - 1].rotation;

                //others
                shadowsLayer.SetActive(false);
                ParticleClouds.SetActive(false);
                upperCloudLayer.SetActive(false);
                cloudLayerA.SetActive(false);
                cloudLayerB.SetActive(false);
                cloudLayerMAIN.SetActive(false);
            }
        }

    }
}
