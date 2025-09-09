// Upgrade NOTE: replaced 'SeperateSpecular' with 'SeparateSpecular'

Shader "Vertex Colored Fog OFF with Distance Fade" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColor ("Spec Color", Color) = (1,1,1,1)
        _Emission ("Emmisive Color", Color) = (0,0,0,0)
        _Shininess ("Shininess", Range (0.01, 1)) = 0.7
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _ViewDistance ("View Distance", Float) = 100.0
        _FadeStart ("Fade Start Distance", Float) = 80.0
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
            float _ViewDistance;
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
                // Sample texture and apply vertex color
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 col = texColor * i.color * _Color;
                
                // Calculate alpha based on distance
                float fadeRange = _ViewDistance - _FadeStart;
                float alpha = 1.0;
                
                if (i.distance > _FadeStart) {
                    // Fade out between fade start and view distance
                    alpha = 1.0 - saturate((i.distance - _FadeStart) / fadeRange);
                }
                
                if (i.distance >= _ViewDistance) {
                    // Completely transparent beyond view distance
                    alpha = 0.0;
                }
                
                // Apply distance-based alpha
                col.a *= alpha;
                
                // Add specular and emission components
                col.rgb += _SpecColor.rgb * _Shininess;
                col.rgb += _Emission.rgb;
                
                return col;
            }
            ENDCG
        }
    }

    Fallback "VertexLit", 1
}