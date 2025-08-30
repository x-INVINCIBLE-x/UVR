using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class CameraFogOverride : MonoBehaviour
{
    private bool prevFog;

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
        RenderPipelineManager.endCameraRendering += EndCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
        RenderPipelineManager.endCameraRendering -= EndCameraRendering;
    }

    private void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == GetComponent<Camera>()) // only this camera
        {
            prevFog = RenderSettings.fog;
            RenderSettings.fog = false;
            // Debug.Log("Fog OFF for " + camera.name);
        }
    }

    private void EndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == GetComponent<Camera>())
        {
            RenderSettings.fog = prevFog;
            // Debug.Log("Fog restored for " + camera.name);
        }
    }
}