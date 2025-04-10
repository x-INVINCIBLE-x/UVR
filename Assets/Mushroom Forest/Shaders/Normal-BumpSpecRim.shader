Shader "URP/BumpedSpecularRim"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColorTexture ("Specular Color",2D) = "black" {}
        _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }
        LOD 400
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 positionHCS : SV_POSITION;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_BumpMap); SAMPLER(sampler_BumpMap);
            TEXTURE2D(_SpecColorTexture); SAMPLER(sampler_SpecColorTexture);
            
            CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float _Shininess;
            float4 _RimColor;
            float _RimPower;
            CBUFFER_END
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.uv = IN.uv;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS);
                OUT.viewDirWS = GetCameraPositionWS() - positionInputs.positionWS;
                OUT.normalWS = normalInputs.normalWS;
                OUT.positionHCS = positionInputs.positionCS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Sample textures
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half4 specColor = SAMPLE_TEXTURE2D(_SpecColorTexture, sampler_SpecColorTexture, IN.uv);
                half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, IN.uv));
                half3 normalWS = TransformTangentToWorld(normalTS, half3x3(IN.normalWS, cross(float3(0,1,0), IN.normalWS), cross(IN.normalWS, float3(0,0,1))));

                // Lighting
                Light mainLight = GetMainLight();
                half3 lightDir = normalize(mainLight.direction);
                half3 viewDir = normalize(IN.viewDirWS);
                
                half rim = 1.0 - saturate(dot(viewDir, normalWS));
                half3 rimLighting = _RimColor.rgb * pow(rim, _RimPower);
                
                // Specular
                half3 halfDir = normalize(lightDir + viewDir);
                half specIntensity = pow(saturate(dot(normalWS, halfDir)), _Shininess * 128.0);
                half3 specular = specColor.rgb * specIntensity * mainLight.shadowAttenuation;
                
                // Final Color
                half3 diffuse = texColor.rgb * _Color.rgb * mainLight.color.rgb * saturate(dot(normalWS, lightDir));
                half3 finalColor = diffuse + specular + rimLighting;
                return half4(finalColor, texColor.a);
            }
            ENDHLSL
        }
    }
}
