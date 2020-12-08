Shader "Custom/WaveField"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorTop ("Color Top", Color) = (1.0, 1.0, 1.0, 1.0)
        _ColorBottom ("Color Bottom", Color) = (1.0, 1.0, 1.0, 1.0)
        _Frequency ("Frequency ", Float) = 1.0
        _Amplitude ("Amplitude", Float) = 0.5
        _Speed ("Speed ",Float) = 1.0
        _ShowHeightThreshold ("Show by Height", Range(0.0, 1.0)) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            fixed4 _ColorTop;
            fixed4 _ColorBottom;
            float _Frequency;
            float _Amplitude;
            float _Speed;
            fixed _ShowHeightThreshold;            

            v2f vert (appdata v)
            {
                v2f o;
                
                float time = _Time * _Speed;
                float offsetY  = sin(time + v.vertex.x * _Frequency) + sin(time + v.vertex.z * _Frequency);
                offsetY         *= _Amplitude;
                offsetY         *= v.uv.y;
                v.vertex.y      += offsetY;   
                             
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = lerp(_ColorTop, _ColorBottom, (_SinTime.w+1.0)*.5);
                
                //fixed4 col = fixed4((_SinTime.w+1.0)*.5, (_SinTime.z+1.0)*.5, (_SinTime.y+1.0)*.5, 0.8);
                
                fixed isShow = i.uv.y > _ShowHeightThreshold ? 1 : 0;
                col.a = lerp(1 - i.uv.y, 0.0, isShow);
                return col;
            }
            ENDCG
        }
    }
}
