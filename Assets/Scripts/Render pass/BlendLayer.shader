Shader "BlendLayer"
{
   SubShader
   {
       Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
       Blend SrcAlpha One
       ZWrite Off Cull Off
       Pass
       {
           Name "BlendLayer"

           HLSLPROGRAM
           #pragma multi_compile_local_fragment _ _USE_FBF
           #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
           #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

           #pragma vertex Vert
           #pragma fragment Frag

#ifdef _USE_FBF
           // Declares the framebuffer input as a texture 2d containing half.
           FRAMEBUFFER_INPUT_HALF(0);
#endif
           // Out frag function takes as input a struct that contains the screen space coordinate we are going to use to sample our texture. It also writes to SV_Target0, this has to match the index set in the UseTextureFragment(sourceTexture, 0, …) we defined in our render pass script.   
           float4 Frag(Varyings input) : SV_Target0
           {
               // this is needed so we account XR platform differences in how they handle texture arrays
               UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

               // read the current pixel from the framebuffer
               float2 uv = input.texcoord.xy;

#ifdef _USE_FBF
               // read previous subpasses directly from the framebuffer.
               half4 color = LOAD_FRAMEBUFFER_INPUT(0, input.positionCS.xy);
#else
               half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
#endif
               // Modify the sampled color
               return color;
           }

           ENDHLSL
       }
   }
}