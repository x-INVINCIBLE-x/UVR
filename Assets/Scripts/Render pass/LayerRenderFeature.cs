using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Experimental.Rendering;

public class LayerRenderFeature : ScriptableRendererFeature
{
    public RenderPassEvent push = RenderPassEvent.AfterRenderingPostProcessing;
    public RenderPassEvent pop = RenderPassEvent.AfterRenderingPostProcessing+49;

    public Material blendMaterial;

    public bool useFBF = false;

    public class StackLayers : ContextItem
    {
        // The texture reference variable.
        public Stack<TextureHandle> layers = new();

        // Reset function required by ContextItem. It should reset all variables not carried
        // over to next frame.
        public override void Reset()
        {
            // We should always reset texture handles since they are only vaild for the current frame.
            layers.Clear();
        }
    }

    class PushLayerRenderPass : ScriptableRenderPass
    {
        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourcesData = frameData.Get<UniversalResourceData>();

            var firstLayer = frameData.Contains<StackLayers>();
            var layers = frameData.GetOrCreate<StackLayers>();

            layers.layers.Push(resourcesData.cameraColor);

            TextureDesc desc;

            //TODO not possible at the moment, needs changes in Unity
            if (resourcesData.isActiveTargetBackBuffer)
            {
                Debug.LogError("Layer Feature doesn't work with direct to backbuffer rendering (yet)");
                var info = renderGraph.GetRenderTargetInfo(resourcesData.backBufferColor);
                desc = new()
                {
                    width = info.width,
                    height = info.height,
                    format = info.format,
                    msaaSamples = MSAASamples.None,
                    bindTextureMS = false,
                    dimension = TextureDimension.Tex2D, //TODO?
                    slices = info.volumeDepth
                };     
            }
            else
            {
                desc = resourcesData.cameraColor.GetDescriptor(renderGraph);
            }

            if (!GraphicsFormatUtility.HasAlphaChannel(desc.format))
            {
                Debug.LogWarning("Layer does not have an alpha channel. Blending will overwrite the entire screen.");
            }

            desc.clearBuffer = true;
            desc.clearColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            desc.name = "_CameraColorLayer" + layers.layers.Count;
            
            var layerColor = renderGraph.CreateTexture(desc);

            resourcesData.cameraColor = layerColor;
        }
    }

    class PopLayerRenderPass : ScriptableRenderPass
    {
        public Material blendMaterial { get; set; }
        public bool useFBF { get; set; }

        protected string m_FBFKeyword = "_USE_FBF";

        // This class stores the data needed by the pass, passed as parameter to the delegate function that executes the pass
        private class PassData
        {
            internal TextureHandle src;
            internal Material material;
        }

        public PopLayerRenderPass()
        {
            profilingSampler = new ProfilingSampler("Blend Layer");
        }

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (blendMaterial != null)
            {
                UniversalResourceData resourcesData = frameData.Get<UniversalResourceData>();

                var firstLayer = frameData.Contains<StackLayers>();
                var layers = frameData.GetOrCreate<StackLayers>();

                if (layers.layers.Count > 0)
                {
                    var previousLayer = layers.layers.Pop();

                    if(useFBF)
                    {
                        blendMaterial.EnableKeyword(m_FBFKeyword);
                        using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
                        {
                            // Fill the pass data
                            passData.material = blendMaterial;

                            // We declare the src as input attachment. This is required for Frame buffer fetch. 
                            builder.SetInputAttachment(resourcesData.cameraColor, 0, AccessFlags.Read);

                            // Setup as a render target via UseTextureFragment, which is the equivalent of using the old cmd.SetRenderTarget
                            builder.SetRenderAttachment(previousLayer, 0);

                            // Assign the ExecutePass function to the render pass delegate, which will be called by the render graph when executing the pass
                            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                            {
                                context.cmd.DrawProcedural(Matrix4x4.identity, data.material, 0, MeshTopology.Triangles, 3, 1, null);
                            });
                        }
                    }
                    else
                    {
                        blendMaterial.DisableKeyword(m_FBFKeyword);
                        RenderGraphUtils.BlitMaterialParameters blitMaterialParameters = new(resourcesData.cameraColor, previousLayer,blendMaterial,0);
                        renderGraph.AddBlitPass(blitMaterialParameters, passName);
                    }

                    //if(previousLayer.Equals(resourcesData.backBufferColor))
                    resourcesData.cameraColor = previousLayer;
                }
            }
        }
    }

    PushLayerRenderPass m_pushPass;
    PopLayerRenderPass m_popPass;

        /// <inheritdoc/>
    public override void Create()
    {
        m_pushPass = new PushLayerRenderPass();
        m_popPass = new PopLayerRenderPass();
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_pushPass.renderPassEvent = push;

        //TODO document this
        m_popPass.renderPassEvent = pop;
        m_popPass.blendMaterial = blendMaterial;
        m_popPass.useFBF = useFBF;


        renderer.EnqueuePass(m_pushPass);
        renderer.EnqueuePass(m_popPass);
    }
}
