using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class VolumeParticleShadePDM : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
    public bool Unity2020 = false;
    public Light Sun;
	public Material Particle_Mat;
	// Update is called once per frame
	void Update () {
		if(Sun != null & Particle_Mat !=null){
			Particle_Mat.SetVector("_SunColor",Sun.color);
			Particle_Mat.SetFloat("_SunLightIntensity",Sun.intensity);
            if (Unity2020)
            {
                Particle_Mat.SetVector("ForwLight", -Sun.transform.forward);
            }
        }
	}
}

