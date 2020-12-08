Shader "Custom/SpriteBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SamplingDistance ("Sampling Distance", float) = 1.0
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.1
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Pass
        {
           CGPROGRAM
           #pragma vertex vert
           #pragma fragment frag
            
           #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half2 coordV : TEXCOORD0;
                half2 coordH : TEXCOORD1;
                float4 vertex : SV_POSITION;
                half2 offsetV: TEXCOORD2;
                half2 offsetH: TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _MainTex_TexelSize;
            float _SamplingDistance;
            float _Cutoff;
            
            static const int samplingCount = 7;
            static const half weights[samplingCount] = { 0.036, 0.113, 0.216, 0.269, 0.216, 0.113, 0.036 };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                half2 uv = TRANSFORM_TEX(v.uv, _MainTex);

                // サンプリングポイントのオフセット
                o.offsetV = _MainTex_TexelSize.xy * half2(0.0, 1.0) * _SamplingDistance;
                o.offsetH = _MainTex_TexelSize.xy * half2(1.0, 0.0) * _SamplingDistance;

                // サンプリング開始ポイントのUV座標
                o.coordV = uv - o.offsetV * ((samplingCount - 1) * 0.5);
                o.coordH = uv - o.offsetH * ((samplingCount - 1) * 0.5);

                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                half4 col = 0;

                // 垂直方向
                for (int j = 0; j < samplingCount; j++){
                    // サンプリングして重みを掛ける。後で水平方向も合成するため0.5をかける
                    col += tex2D(_MainTex, i.coordV) * weights[j] * 0.5;
                    // offset分だけサンプリングポイントをずらす
                    i.coordV += i.offsetV;
                }

                // 水平方向
                for (int j = 0; j < samplingCount; j++){
                    col += tex2D(_MainTex, i.coordH) * weights[j] * 0.5;
                    i.coordH += i.offsetH;
                }
                
                clip(col.a - _Cutoff);
                return col;
            }
            ENDCG
        }
    }
}
