Shader "Custom/VisualEffects/Dissolve"
{
    Properties
    {
        [HideInInspector] _MainTex ("MainTex", 2D) = "white" {}
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
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Dissolve"
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Assets/VisualEffects/Common.cginc"
            

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _DissolveTex;
            float _Threshold;
            
            fixed3 _GlowColor;
            half _GlowRange;
            half _GlowFalloff;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float dissolve = tex2D(_DissolveTex, i.uv).r;
                dissolve = dissolve * 0.999;
                float isVisible = dissolve - _Threshold;
                clip(isVisible);

                float isGlowing = smoothstep(_GlowRange + _GlowFalloff, _GlowRange, isVisible);
                float3 glow = isGlowing * _GlowColor;    
    
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                if (_Threshold > 0.001) {
                    return fixed4(col.r + glow.x, col.g + glow.y, col.b + glow.z, col.a);
                }
                return col;
    
            }

            ENDCG
        }
    }
}