Shader "Hidden/VolumetricFog"
{
    SubShader
    {
        Tags
        { 
            "RenderPipeline" = "UniversalPipeline"
        }

        // ---------------- Volumetric Fog Render ----------------
        Pass
        {
            Name "VolumetricFogRender"

            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM

            #include "./VolumetricFog.hlsl"

            // ===== Forward+ / Clustered lighting support =====
            #ifndef UNIVERSAL_FORWARD_PLUS_KEYWORD_DEPRECATED_INCLUDED
            #define UNIVERSAL_FORWARD_PLUS_KEYWORD_DEPRECATED_INCLUDED
            #if defined(_FORWARD_PLUS)
                #warning "_FORWARD_PLUS shader keyword deprecated, use _CLUSTER_LIGHT_LOOP."
                #define USE_FORWARD_PLUS USE_CLUSTER_LIGHT_LOOP
                #define FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK CLUSTER_LIGHT_LOOP_SUBTRACTIVE_LIGHT_CHECK
                #ifndef _CLUSTER_LIGHT_LOOP
                    #define _CLUSTER_LIGHT_LOOP 1
                #endif
            #endif
            #endif
            // ===================================================

            #pragma multi_compile _ USE_FORWARD_PLUS USE_CLUSTER_LIGHT_LOOP FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK CLUSTER_LIGHT_LOOP_SUBTRACTIVE_LIGHT_CHECK
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
#if UNITY_VERSION >= 202310
            #pragma multi_compile_fragment _ PROBE_VOLUMES_L1 PROBE_VOLUMES_L2
#endif
            #pragma multi_compile_local_fragment _ _MAIN_LIGHT_CONTRIBUTION_DISABLED
            #pragma multi_compile_local_fragment _ _ADDITIONAL_LIGHTS_CONTRIBUTION_DISABLED
            #pragma multi_compile_local_fragment _ _APV_CONTRIBUTION_ENABLED

            #pragma vertex Vert
            #pragma fragment Frag

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return VolumetricFog(input.texcoord, input.positionCS.xy);
            }

            ENDHLSL
        }

        // ---------------- Horizontal Blur ----------------
        Pass
        {
            Name "VolumetricFogHorizontalBlur"

            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "./DepthAwareGaussianBlur.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // _BlitTexture and sampler_PointClamp provided automatically by URP
                return DepthAwareGaussianBlur(input.texcoord, float2(1.0, 0.0), _BlitTexture, sampler_PointClamp, _BlitTexture_TexelSize.xy);
            }

            ENDHLSL
        }

        // ---------------- Vertical Blur ----------------
        Pass
        {
            Name "VolumetricFogVerticalBlur"

            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "./DepthAwareGaussianBlur.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return DepthAwareGaussianBlur(input.texcoord, float2(0.0, 1.0), _BlitTexture, sampler_PointClamp, _BlitTexture_TexelSize.xy);
            }

            ENDHLSL
        }

        // ---------------- Upsample ----------------
        Pass
        {
            Name "VolumetricFogUpsampleComposition"

            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "./DepthAwareUpsample.hlsl"

            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            // Must explicitly declare this texture
            TEXTURE2D_X(_VolumetricFogTexture);

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 volumetricFog = DepthAwareUpsample(input.texcoord, _VolumetricFogTexture);
                float4 cameraColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, input.texcoord);

                return float4(cameraColor.rgb * volumetricFog.a + volumetricFog.rgb, cameraColor.a);
            }

            ENDHLSL
        }
    }

    Fallback Off
}
