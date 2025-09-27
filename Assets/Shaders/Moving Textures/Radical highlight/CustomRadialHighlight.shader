Shader "Custom/RadialHighlight"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _HighlightColor ("Highlight Color", Color) = (1,1,1,1)
        _AngleStart ("Start Angle", Range(0,360)) = 0
        _AngleEnd ("End Angle", Range(0,360)) = 90
        _InnerRadius ("Inner Radius", Range(0,1)) = 0.3
        _OuterRadius ("Outer Radius", Range(0,1)) = 0.35
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _HighlightColor;
            float _AngleStart, _AngleEnd;
            float _InnerRadius, _OuterRadius;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float Angle360(float2 uv)
            {
                float angle = atan2(uv.y, uv.x) * 57.2958; // radians -> degrees
                return (angle < 0) ? angle + 360 : angle;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 centeredUV = i.uv - 0.5;
                float dist = length(centeredUV);

                // angle in [0,360]
                float angle = Angle360(centeredUV);

                // check if inside angle range
                float angleMask = step(_AngleStart, angle) * step(angle, _AngleEnd);

                // check if inside ring
                float ringMask = step(_InnerRadius, dist) * step(dist, _OuterRadius);

                // combine
                float mask = angleMask * ringMask;

                fixed4 baseCol = tex2D(_MainTex, i.uv);
                return baseCol + _HighlightColor * mask;
            }
            ENDCG
        }
    }
}
