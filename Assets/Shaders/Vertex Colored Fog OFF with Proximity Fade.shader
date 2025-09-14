// Upgrade NOTE: replaced 'SeperateSpecular' with 'SeparateSpecular'

Shader "Vertex Colored Fog OFF with Proximity Fade" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColor ("Spec Color", Color) = (1,1,1,1)
        _Emission ("Emmisive Color", Color) = (0,0,0,0)
        _Shininess ("Shininess", Range (0.01, 1)) = 0.7
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _FadeDistance ("Fade Distance", Float) = 2.0
        _FadeStart ("Fade Start Distance", Float) = 5.0
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200
        
        Pass {
            Fog {Mode Off}
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
                float distance : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _SpecColor;
            float4 _Emission;
            float _Shininess;
            float _FadeDistance;
            float _FadeStart;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                
                // Calculate distance from camera to vertex
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 cameraPos = _WorldSpaceCameraPos;
                o.distance = distance(worldPos, cameraPos);
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                // Sample texture and apply vertex color (keep original RGB values)
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 col = texColor * i.color * _Color;
                
                // Add specular and emission components (keep original RGB values)
                col.rgb += _SpecColor.rgb * _Shininess;
                col.rgb += _Emission.rgb;
                
                // Only modify alpha based on proximity
                float fadeRange = _FadeStart - _FadeDistance;
                float alpha = 1.0;
                
                if (i.distance < _FadeStart) {
                    // Fade out as player gets closer (only affects alpha)
                    alpha = saturate((i.distance - _FadeDistance) / fadeRange);
                }
                
                if (i.distance <= _FadeDistance) {
                    // Completely transparent when very close (only affects alpha)
                    alpha = 0.0;
                }
                
                // Apply proximity-based alpha only
                col.a = alpha;
                
                return col;
            }
            ENDCG
        }
    }

    Fallback "VertexLit", 1
}