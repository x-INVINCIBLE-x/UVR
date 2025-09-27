Shader "Custom/URP3DRingRadialHighlight"
{
    Properties
    {
        _BaseMap("Base Texture", 2D) = "white" {}
        _HighlightColor("Highlight Color", Color) = (1,1,0,1)
        _AngleStart("Start Angle", Range(0,360)) = 0
        _AngleEnd("End Angle", Range(0,360)) = 90
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _HighlightColor;
            float _AngleStart, _AngleEnd;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv         : TEXCOORD0;
                float3 worldPos   : TEXCOORD1;
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.uv = IN.uv;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            float Angle360(float2 v)
            {
                float angle = atan2(v.x, v.y) * 57.2958; // radians to degrees
                return (angle < 0) ? angle + 360 : angle;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Base texture sample
                float4 baseCol = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

                // XZ projection
                float2 posXZ = float2(IN.worldPos.x, IN.worldPos.z);

                // angle
                float angle = Angle360(normalize(posXZ));

                // angle mask
                float angleMask = step(_AngleStart, angle) * step(angle, _AngleEnd);

                // mix
                float3 finalColor = baseCol.rgb + _HighlightColor.rgb * angleMask;
                return float4(finalColor, baseCol.a);
            }
            ENDHLSL
        }
    }
}
