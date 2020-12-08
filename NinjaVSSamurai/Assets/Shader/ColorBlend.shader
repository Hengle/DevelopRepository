Shader "Custom/ColorBlend"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _BlendTex ("AlphaMask", 2D) = "white" {}
        _MaskTex ("MaskTex", 2D) = "white" {}
        _Color ("color", Color) = (1,1,1,1)
        
        [KeywordEnum(NOTHING, ADDITIVE, MULTIPLY, SCREEN, OVERLAY)]
        _Blend ("Color Blend", Float) = 0
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile _BLEND_NOTHING _BLEND_ADDITIVE _BLEND_MULTIPLY _BLEND_SCREEN _BLEND_OVERLAY
            
            #include "UnityCG.cginc"
            
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _BlendTex;
            sampler2D _MaskTex;
            fixed4 _Color;
            
            fixed4 CalcAdditiveColor (fixed4 baseColor, fixed4 blendColor)
            {
                return fixed4(baseColor.rgb + blendColor.rgb, baseColor.a);
            }
            
            fixed4 CalcMultiplyColor (fixed4 baseColor, fixed4 blendColor)
            {
                return fixed4(baseColor.rgb * blendColor.rgb, baseColor.a);
            }
            
            fixed4 CalcScreenColor (fixed4 baseColor, fixed4 blendColor)
            {
                return fixed4(1 - (1.0 - baseColor.rgb) * (1.0 - blendColor.rgb), baseColor.a);
            }
            
            fixed4 CalcOverlayColor (fixed4 baseColor, fixed4 blendColor)
            {
                fixed4 multiplyColor = CalcMultiplyColor(baseColor, blendColor);
                
                fixed overlayR = baseColor.r >= 0.5 ? 1.0 - 2.0 * (1.0 - baseColor.r) * (1.0 - blendColor.r) : 2.0 * multiplyColor.r;
                fixed overlayG = baseColor.g >= 0.5 ? 1.0 - 2.0 * (1.0 - baseColor.g) * (1.0 - blendColor.g) : 2.0 * multiplyColor.g;
                fixed overlayB = baseColor.b >= 0.5 ? 1.0 - 2.0 * (1.0 - baseColor.b) * (1.0 - blendColor.b) : 2.0 * multiplyColor.b;
                return fixed4(overlayR, overlayG, overlayB, baseColor.a);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                fixed4 maskColor = tex2D(_MaskTex, i.uv);
                fixed4 blendColor = tex2D(_BlendTex, i.uv);
                
                #ifdef _BLEND_ADDITIVE
                blendColor = CalcAdditiveColor(color, blendColor);
                #elif _BLEND_MULTIPLY
                blendColor = CalcMultiplyColor(color, blendColor);
                #elif _BLEND_SCREEN
                blendColor = CalcScreenColor(color, blendColor);
                #elif _BLEND_OVERLAY
                blendColor = CalcOverlayColor(color, blendColor);
                #elif _BLEND_NOTHING
                blendColor = blendColor;
                #endif
                
                return lerp(color, blendColor, maskColor.a);
            }
            ENDCG
        }
    }
}
