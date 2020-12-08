Shader "Custom/VisualEffects/Twinkle"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        
        _TwinkleTex ("TwinkleTex", 2D) = "white" {}
        _EffectPos ("_EffectPos", Range(-1, 1)) = 1
        
        [KeywordEnum(ADDITIVE, MULTIPLY, SCREEN, OVERLAY)]
        _Blend ("Color Blend", Float) = 0
    }
    
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _BLEND_ADDITIVE _BLEND_MULTIPLY _BLEND_SCREEN _BLEND_OVERLAY
            
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
            
            sampler2D _TwinkleTex;
            float _EffectPos;
            
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
                fixed2 effectUV = i.uv;
                effectUV += _EffectPos;
                
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 sub = tex2D(_TwinkleTex, effectUV);
                
                #ifdef _BLEND_ADDITIVE
                col.rgb = lerp(col.rgb, CalcAdditiveColor(col, sub).rgb, sub.a);
                #elif _BLEND_MULTIPLY
                col.rgb = lerp(col.rgb, CalcMultiplyColor(col, sub).rgb, sub.a);
                #elif _BLEND_SCREEN
                col.rgb = lerp(col.rgb, CalcScreenColor(col, sub).rgb, sub.a);
                #elif _BLEND_OVERLAY
                col.rgb = lerp(col.rgb, CalcOverlayColor(col, sub).rgb, sub.a);
                #endif
                
                return col * i.color;
            }
            ENDCG
        }
    }
}
