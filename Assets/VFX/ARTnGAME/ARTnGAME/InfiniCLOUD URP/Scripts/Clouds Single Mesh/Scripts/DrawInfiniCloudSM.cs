using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Artngame.SKYMASTER
{
    [ExecuteAlways] //
   // [ExecuteInEditMode]
    public class DrawInfiniCloudSM : MonoBehaviour
    {
        private Material cloudsMat;
        private Transform thisTransform;
        public Camera cameraMain;

        public int layer = 30;

        private Matrix4x4 rotTraM;
        private Matrix4x4[] cloudsMats;
        private float offset = 0.005f;

        public Mesh meshCLoudsStack;
        public bool drawCloudsStack = true;
        public float cloudsHeight = 3f;
        [Range(0, 300)] public int cloudStackDensity = 20;
        private int cloudDensityApplied;

        public bool followTransform = true;
        public Transform cameraTransform;
        private Vector3 followPosition;

        public bool castCloudShadows = false;
        public bool castMainShadow = true;
        public bool recieveShadows = false;

        public int minimumCloudDensity = 1;
        public float distanceCloudsFade = 20f;

        public float closeupFade = 1f;
        public bool useGpuInstance = false;
        public bool reverseDrawOrder = false;

        public bool updateCloudsConstantly = false;

        //v0.2
        //https://docs.unity3d.com/Manual/UIE-Keyboard-Events.html

        private void OnEnable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += MyMethod;
#endif
        }

        void MyMethod()
        {

            //v0.5
//            if (liveUpdateSceneViewCamera)
//            {
//#if UNITY_EDITOR
//                SceneView sceneCam = SceneView.lastActiveSceneView;
//                if (sceneCam != null)
//                {
//                    sceneCam.camera.transform.position = Camera.main.transform.position;
//                    sceneCam.camera.transform.rotation = Camera.main.transform.rotation;
//                    sceneCam.AlignViewToObject(Camera.main.transform);
//                }
//#endif
//            }

            //v0.4
            if (updateCloudsConstantly && !Application.isPlaying)
            {
                Camera.main.transform.position += new Vector3(0, 0, 0.1f);
                Camera.main.transform.position += new Vector3(0, 0, -0.1f);
            }
           
        }



        void Start()
        {
            if (thisTransform == null) { 
                thisTransform = transform;
            }
            if (meshCLoudsStack == null) { 
                meshCLoudsStack = GetComponent<MeshFilter>().sharedMesh;
            }
            if (cameraMain == null) { 
                cameraMain = Camera.main;
            }
            if (!cameraTransform) { 
                cameraTransform = cameraMain.transform;
            }
            GetComponent<Renderer>().enabled = false;            
            cloudsMat = GetComponent<Renderer>().sharedMaterial;            
            rotTraM = Matrix4x4.TRS(thisTransform.position + (offset * Vector3.up), thisTransform.rotation, thisTransform.localScale);
        }

        void OnDrawGizmos()
        {

#if UNITY_EDITOR
            // Your gizmo drawing thing goes here if required...
            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }

            // Ensure continuous Update calls.
            if (!Application.isPlaying)
            {
                Update();
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            }
#endif
        }

        void OnGUI()
        {
            // Your gizmo drawing thing goes here if required...

#if UNITY_EDITOR
            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }

            // Ensure continuous Update calls.
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();

                if (Event.current.type == EventType.MouseMove)
                {
                    //Repaint();
                    Update();
                    UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                    UnityEditor.SceneView.RepaintAll();
                }
            }
#endif
        }

        void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                Update();
            }
        }


        public void Update()
        {
            cloudsMat.SetFloat("_midYValue", thisTransform.position.y);
            cloudsMat.SetFloat("_cloudHeight", cloudsHeight);
            float heightDifference = Mathf.Abs(thisTransform.position.y - cameraTransform.position.y);
            float floorMultiplier = 1.0f / (1.0f - closeupFade); // so that when it gets close it fades in fully

            cloudDensityApplied = 0;
            if (heightDifference < distanceCloudsFade)
            {
                cloudDensityApplied = (int)(cloudStackDensity * Mathf.Clamp01(((1f - (heightDifference / distanceCloudsFade)) * floorMultiplier)));
            }

            if (cloudDensityApplied < minimumCloudDensity)
            {
                minimumCloudDensity = Mathf.Clamp(minimumCloudDensity, 0, cloudStackDensity);
                cloudDensityApplied = minimumCloudDensity;
            }

            if (!Application.isPlaying || ( drawCloudsStack && cloudDensityApplied != 0))
            {
                DrawClouds();
            }

            if (cameraTransform != null)
            {
                followPosition = cameraTransform.position;
                followPosition.y = thisTransform.position.y;
                if (followTransform)
                {
                    thisTransform.position = followPosition;
                }
            }
        }
        void DrawClouds()
        {
            if (meshCLoudsStack == null)
            {
                return;
            }

            offset = cloudsHeight / cloudDensityApplied / 2f;
            rotTraM = Matrix4x4.TRS(thisTransform.position, thisTransform.rotation, thisTransform.localScale);

            if (useGpuInstance)
            {
                cloudsMats = new Matrix4x4[cloudDensityApplied]; 
            }
            else
            {
                Graphics.DrawMesh(meshCLoudsStack, rotTraM, cloudsMat, layer, cameraMain, 0, null, castMainShadow, recieveShadows, false);
            }

            if (reverseDrawOrder)
            {
                Vector3 startPosition = thisTransform.position + ((offset * Vector3.up * cloudDensityApplied / 2f));
                for (int i = 0; i < cloudDensityApplied; i++)
                {
                    rotTraM = Matrix4x4.TRS(startPosition - (offset * Vector3.up * offset * i), thisTransform.rotation, thisTransform.localScale);

                    if (useGpuInstance)
                    {
                        cloudsMats[i] = rotTraM;                        
                    }
                    else
                    {
                        Graphics.DrawMesh(meshCLoudsStack, rotTraM, cloudsMat, layer, cameraMain, 0, null, castCloudShadows, recieveShadows, false);
                    }
                }

            }
            else
            {

                for (int i = 1; i <= cloudDensityApplied; i++)
                {
                    rotTraM = Matrix4x4.TRS(thisTransform.position + (offset * Vector3.up * i), thisTransform.rotation, thisTransform.localScale);
                    if (useGpuInstance)
                    {
                        cloudsMats[i - 1] = rotTraM;                        
                    }
                    else
                    {
                        Graphics.DrawMesh(meshCLoudsStack, rotTraM, cloudsMat, layer, cameraMain, 0, null, castCloudShadows, recieveShadows, false);
                    }
                    offset *= -1;
                }
            }

            if (useGpuInstance)
            {
                UnityEngine.Rendering.ShadowCastingMode shadowCasting = UnityEngine.Rendering.ShadowCastingMode.Off;
                if (castCloudShadows)
                {
                    shadowCasting = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                Graphics.DrawMeshInstanced(meshCLoudsStack, 0, cloudsMat, cloudsMats,
                    cloudDensityApplied, null, shadowCasting, recieveShadows, layer, 
                    cameraMain);
            }

        }
    }
}