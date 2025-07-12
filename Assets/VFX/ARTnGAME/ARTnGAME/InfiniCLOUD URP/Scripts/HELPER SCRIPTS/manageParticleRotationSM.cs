using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Artngame.SKYMASTER
{
    public class manageParticleRotationSM : MonoBehaviour
    {
        //v3.4.6
        ParticleSystem ShurikenParticle;
        ParticleSystemRenderer PrenderS;
        public bool StableRollMethod2 = false;
        public bool StableRollAllAxis = false;
        int psize = 0;
        // Start is called before the first frame update
        void Start()
        {
            Cam_transf = Camera.main.transform;
            Prev_cam_rot = Cam_transf.eulerAngles;

            ParticleSystem.MainModule MainMod = GetComponent<ParticleSystem>().main; //v3.4.9
            psize = (int)MainMod.maxParticles; //v3.4.9

            int SizeP = MainMod.maxParticles;

            //int SizeP = ((int)(psize / divider)) * (divider);
            //MainMod.maxParticles = SizeP; //+ (divider)*1;
            //MainMod.maxParticles = SizeP; //+ (divider)*1;
            //}
            //		GetComponent<ParticleSystem> ().ClearParticles ();
            //GetComponent<ParticleSystem>().Emit(SizeP);
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[SizeP];
            GetComponent<ParticleSystem>().GetParticles(particles); //		Particle[] particles = GetComponent<ParticleEmitter> ().particles;

            //v1.7 - init clouds on start
            //if (GetComponent<ParticleSystem>() != null)
            //{

            //v3.5
            if (this.GetComponent<ParticleSystem>() != null)
            {
                //ScatterMat = this.GetComponent<ParticleSystemRenderer>().material;
                PrenderS = this.gameObject.GetComponent<ParticleSystemRenderer>();
                ShurikenParticle = this.GetComponent<ParticleSystem>();
            }
            if (PrenderS != null)
            {
                PrenderS.sortMode = ParticleSystemSortMode.Distance;// = ParticleRenderMode.SortedBillboard;
                                                                    //PrenderS.renderMode = ParticleSystemRenderMode.Billboard;
            }

        }

        Transform Cam_transf;
        Vector3 Prev_cam_rot;

        // Update is called once per frame
        void LateUpdate()
        {
            //v3.1
            float currentRot = Cam_transf.eulerAngles.z;
            float diffRot = currentRot - Prev_cam_rot.z;

            float currentRotY = Cam_transf.eulerAngles.y;
            float diffRotY = currentRotY - Prev_cam_rot.y;

            if (Mathf.Abs(diffRotY) > 180)
            {
                diffRotY = 360 - Mathf.Abs(diffRotY);
            }
            float Angle_check = 0;
            float Dot_check = 0;
            //v3.1
            if (StableRollMethod2)
            {
                Angle_check = Mathf.Abs(Vector3.Angle(Cam_transf.forward, Mathf.Sign(Cam_transf.forward.y) * Vector3.up));
                Dot_check = -Mathf.Sign(Cam_transf.forward.y) * Vector3.Dot(Cam_transf.forward, Mathf.Sign(Cam_transf.forward.y) * Vector3.up);
                //Debug.Log("diffRotY="+diffRotY + "Dot_check="+Dot_check + " SUM="+(diffRotY*Dot_check).ToString() + " Angle="+Angle_check);
            }
            if (particles == null)
            {
                particles = new ParticleSystem.Particle[ShurikenParticle.main.maxParticles]; //v3.4.9
            }
            ShurikenParticle.GetParticles(particles);
            for (int i = 0; i < particles.Length; i = i + 1)
            {
                ////////////////
                //v3.0.2
                if (StableRollMethod2)
                {
                    if (!StableRollAllAxis)
                    {
                        float currentRot1 = Cam_transf.eulerAngles.z;
                        float diffRot1 = currentRot1 - Prev_cam_rot.z;
                        float currentRotY1 = Cam_transf.eulerAngles.y;
                        if (diffRot1 == 0 && currentRotY1 != Prev_cam_rot.y && Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up)) < 65)
                        {//Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up))< 65){
                            float diffRotY1 = currentRotY1 - Prev_cam_rot.y;
                            particles[i].rotation = particles[i].rotation + diffRotY1;
                            //Debug.Log("aa");
                        }
                        else
                        {
                            if (Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up)) < 45)
                            {
                                //particles[i].rotation = Mathf.Lerp(particles[i].rotation,particles[i].rotation+diffRot, Time.deltaTime*2);
                            }
                            else
                            {
                                particles[i].rotation = particles[i].rotation + diffRot1;
                            }
                        }
                    }
                    else
                    {
                        //v3.4.9 - disabled old code below
                        //											if(diffRot == 0 && currentRotY!=Prev_cam_rot.y && Angle_check < 65){//Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up))< 65){									
                        //
                        //												//particles[i].rotation = particles[i].rotation-diffRotY*Dot_check;
                        //												particles[i].rotation = particles[i].rotation+diffRotY*Dot_check * 1; //v3.5
                        //												//Debug.Log("aa");
                        //											}else{
                        //												if(Angle_check < 45){
                        //													//particles[i].rotation = Mathf.Lerp(particles[i].rotation,particles[i].rotation+diffRot, Time.deltaTime*2);
                        //													//particles[i].rotation = particles[i].rotation+diffRot-diffRotY*Dot_check;
                        //												}else{
                        //													//particles[i].rotation = particles[i].rotation+diffRot-diffRotY*Dot_check;
                        //												}
                        //												particles[i].rotation = particles[i].rotation-diffRot+diffRotY*Dot_check  * 1; //v3.5 
                        //											}

                        //v3.4.9
                        //float currentRot1 = Cam_transf.eulerAngles.z;
                        //float diffRot1 = currentRot1 - Prev_cam_rot.z;

                        //float currentRotY1 = Cam_transf.eulerAngles.y;
                        if (diffRot == 0 && currentRotY != Prev_cam_rot.y && Angle_check < 65)
                        {//Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up))< 65){

                            //float diffRotY1 = currentRotY1 - Prev_cam_rot.y;
                            particles[i].rotation = particles[i].rotation - diffRotY * Dot_check;
                            //Debug.Log("aa");
                        }
                        else
                        {
                            if (Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up)) < 45)
                            {
                                //particles[i].rotation = Mathf.Lerp(particles[i].rotation,particles[i].rotation+diffRot, Time.deltaTime*2);
                            }
                            else
                            {
                                //particles[i].rotation = particles[i].rotation+diffRot1-diffRotY*Dot_check ;
                            }
                            particles[i].rotation = particles[i].rotation + diffRot - diffRotY * Dot_check;
                        } //end v3.4.9
                    }
                }

                if (Prev_cam_rot == Cam_transf.eulerAngles)
                {
                    //particles[i].startColor = Color.Lerp(SunColor, MainColor, Diff);

                }
                else
                {
                    Prev_cam_rot = Cam_transf.eulerAngles;
                }
                ////////////////
            }//END PARTILE LOOP

            if (Prev_cam_rot != Cam_transf.eulerAngles)
            {
                Prev_cam_rot = Cam_transf.eulerAngles;
            }
            ShurikenParticle.SetParticles(particles, particles.Length); //v3.5
        }

        ParticleSystem.Particle[] particles;
    }
}