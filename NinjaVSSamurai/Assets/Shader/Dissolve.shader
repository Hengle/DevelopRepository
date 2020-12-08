Shader "Custom/Dissolve"
{
    Properties
    {
        [HideInInspector] _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _DissolveTex ("DissolveTex", 2D) = "white" {}
        _Threshold("Threshold", Range(0,1))= 0.0
        
        [Header(Glow)]
        [HDR]_GlowColor("Color", Color) = (1, 1, 1, 1)
        _GlowRange("Range", Range(0, .5)) = 0.1
        _GlowFalloff("Falloff", Range(0, 1)) = 0.1        
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Dissolve"
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 dissolveUV: TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _DissolveTex;
            float4 _DissolveTex_ST;
            float _Threshold;
            
            float3 _GlowColor;
            float _GlowRange;
            float _GlowFalloff;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.dissolveUV = TRANSFORM_TEX(v.uv, _DissolveTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float dissolve = tex2D(_DissolveTex, i.dissolveUV).r;
                //dissolve = dissolve * 0.999;
                float isVisible = dissolve - _Threshold;
                clip(isVisible);

                float isGlowing = smoothstep(_GlowRange + _GlowFalloff, _GlowRange, isVisible);
                float3 glow = isGlowing * _GlowColor;    
    
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                if (_Threshold > 0.001) {
                    return fixed4(col.r + glow.x, col.g + glow.y, col.b + glow.z, col.a);
                }
                return col;
    
            }

            ENDCG
        }
    }
}