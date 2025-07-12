using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Artngame.SKYMASTER
{
    [CustomEditor(typeof(CloudHandlerSM_SRP))]
    public class CloudHandlerSM_SRPEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            CloudHandlerSM_SRP clouds = (CloudHandlerSM_SRP)target;

            //PARAMETERS
            Undo.RecordObject(clouds, "Clouds changed");
            
            clouds.Update();
            //Undo.RecordObject(clouds.cloudSidesCMat, "Clouds Material changed");

            ///////////////////// CLOUD POSITION - SCALE/////////////////////  
            EditorGUILayout.HelpBox("-------------------------- CLOUD POSITION - SCALE ------------------------", MessageType.None);           
            EditorGUILayout.HelpBox("Positions the height of the clouds in the scene. The shader heigth should be same as actual height start height and " +
                "can be fine tuned from that value to get the required look.", MessageType.Info);

            float cloudHeight = EditorGUILayout.FloatField("Clouds Start Height", clouds.MultiQuadCHeights.x);//, GUILayout.Width(250));
            float cloudShaderHeight = EditorGUILayout.FloatField("Clouds Shader Height", clouds.MultiQuadCHeights.y);//, GUILayout.Width(250));
            clouds.MultiQuadCHeights = new Vector3(cloudHeight, cloudShaderHeight, 0);

            float cloudHorizScale = EditorGUILayout.FloatField("Clouds Horizontal Scale", clouds.MultiQuadCScale.x);//, GUILayout.Width(250));
            float cloudVertScale = EditorGUILayout.FloatField("Clouds Thickness", clouds.MultiQuadCScale.y);//, GUILayout.Width(250));
            clouds.MultiQuadCScale = new Vector3(cloudHorizScale, cloudVertScale, cloudHorizScale);

            clouds.CloudDensity = EditorGUILayout.FloatField("Clouds Tiling Scale", clouds.CloudDensity);

            EditorGUILayout.HelpBox("-----------------------------------------------------------------------------", MessageType.None);



            ///////////////////// CLOUD COVERAGE /////////////////////  
            EditorGUILayout.HelpBox("----------------------------- CLOUD COVERAGE ---------------------------", MessageType.None);
            EditorGUILayout.HelpBox("Control the coverage of clouds, the horizon coverage and the cloud transparency.", MessageType.Info);
           
            clouds.Coverage = EditorGUILayout.FloatField("Clouds coverage", clouds.Coverage);
            clouds.Horizon = EditorGUILayout.FloatField("Clouds horizon density", clouds.Horizon);
            clouds.Transp = EditorGUILayout.FloatField("Clouds transparency", clouds.Transp);

            EditorGUILayout.HelpBox("-----------------------------------------------------------------------------", MessageType.None);



            ///////////////////// CLOUD RENDERING /////////////////////  
            EditorGUILayout.HelpBox("----------------------------- CLOUD RENDERING ---------------------------", MessageType.None);
            EditorGUILayout.HelpBox("Control the rendering of clouds and shadowing", MessageType.Info);

            clouds.IntensityDiffOffset = EditorGUILayout.FloatField("Upper Color Contrast", clouds.IntensityDiffOffset);
            clouds.IntensitySunOffset = EditorGUILayout.FloatField("Sun Power", clouds.IntensitySunOffset);
            clouds.IntensityFogOffset = EditorGUILayout.FloatField("Fog intensity", clouds.IntensityFogOffset);

            clouds.DayCloudColor = EditorGUILayout.ColorField("Upper Cloud Color", clouds.DayCloudColor);
            clouds.DayCloudShadowCol = EditorGUILayout.ColorField("Lower Cloud Color", clouds.DayCloudShadowCol);

            //MASK TEXTURE
            //EditorGUILayout.ObjectField(clouds.cloudSidesCMat.GetTexture("_PaintMap"));
            //GUILayout.Label(clouds.cloudSidesCMat.GetTexture("_PaintMap"));
            //EditorGUI.ObjectField(new Rect(0,0,30,30), "Mask Texture", clouds.cloudSidesCMat.GetTexture("_PaintMap"), typeof(Texture2D),false);
            Texture2D maskTex = (Texture2D)EditorGUILayout.ObjectField("Mask Texture", clouds.cloudSidesCMat.GetTexture("_PaintMap"), typeof(Texture2D), false);
            if (maskTex != clouds.cloudSidesCMat.GetTexture("_PaintMap"))
            {
                Undo.RecordObject(clouds.cloudSidesCMat, "Clouds Material changed");
                EditorUtility.SetDirty(clouds);
            }

         
            clouds.cloudSidesCMat.SetTexture("_PaintMap", maskTex);
            Vector4 scaleAndOffset = clouds.cloudSidesCMat.GetVector("_PaintMap_ST");
            Vector4 scaleAndOffsetM = EditorGUILayout.Vector4Field("Mask Scale-Offset", scaleAndOffset);            
            //set direty if changed to keep the previous material state
            if (   scaleAndOffset.x != scaleAndOffsetM.x || scaleAndOffset.y != scaleAndOffsetM.y 
                || scaleAndOffset.z != scaleAndOffsetM.z || scaleAndOffset.w != scaleAndOffsetM.w )
            {
                //Debug.Log("IN");
                //Undo.RecordObject(clouds.cloudSidesCMat, "Clouds Material changed");
                Undo.RecordObject(clouds.cloudSidesCMat, "Clouds Material changed");
                EditorUtility.SetDirty(clouds);
            }
            // clouds.cloudSidesCMat.SetVector("_PaintMap_ST", scaleAndOffsetM);
            clouds.cloudSidesCMat.SetTextureOffset("_PaintMap", new Vector2(scaleAndOffsetM.z, scaleAndOffsetM.w));
            clouds.cloudSidesCMat.SetTextureScale("_PaintMap", new Vector2(scaleAndOffsetM.x, scaleAndOffsetM.y));
            //Debug.Log(clouds.cloudSidesCMat.GetVector("_PaintMap_ST"));
            EditorGUILayout.HelpBox("-------------------------------------------------------------------------------", MessageType.None);

            ///////////////////// CLOUD VORTEX /////////////////////  
            EditorGUILayout.HelpBox("------------------------------- CLOUD VORTEX -----------------------------", MessageType.None);
            EditorGUILayout.HelpBox("Control the Vortex scale and rotation.", MessageType.Info);

            clouds.vortexVorticity = EditorGUILayout.FloatField("Vortex Power", clouds.vortexVorticity);
            clouds.vortexSpeed = EditorGUILayout.FloatField("Vortex Speed", clouds.vortexSpeed);
            clouds.vortexThickness = EditorGUILayout.FloatField("Vortex Thickness", clouds.vortexThickness);
            clouds.vortexCutoff = EditorGUILayout.FloatField("Vortex Radius", clouds.vortexCutoff);
            clouds.vortexPosition = EditorGUILayout.Vector3Field("Vortex Position", clouds.vortexPosition);

            EditorGUILayout.HelpBox("-----------------------------------------------------------------------------", MessageType.None);

            ///////////////////// CLOUD WEATHER CONTROLS /////////////////////  
            EditorGUILayout.HelpBox("------------------------ CLOUD WEATHER CONTROLS ----------------------", MessageType.None);
            EditorGUILayout.HelpBox("Positions the height of the clouds in the scene", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable Weather"))
            {
                clouds.WeatherDensity = true;
            }
            if (GUILayout.Button("Disable Weather"))
            {
                clouds.WeatherDensity = false;
            }
            EditorGUILayout.EndHorizontal();

            //LIGHTNING
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable Lightning"))
            {
                clouds.EnableLightning = true;
            }
            if (GUILayout.Button("Disable Lightning"))
            {
                clouds.EnableLightning = false;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("-----------------------------------------------------------------------------", MessageType.None);




          




            EditorGUILayout.HelpBox("---------------------- ADVANCED CLOUD CONTROLS ---------------------", MessageType.None);
            DrawDefaultInspector();
        }








        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}