Shader "Custom/TutorialLine"
{
	Properties
	{
		[HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
        _FirstColor ("FirstColor", Color) = (1,1,1,1)
        _SecondColor ("SecondColor", Color) = (1,1,1,1)
        _LerpSpeed ("LerpSpeed", Range (0.0, 1.0)) = 0.0
	}
    
	SubShader
	{
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

        Blend SrcAlpha OneMinusSrcAlpha

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
            
            sampler2D _MaskTex;
            fixed4 _FirstColor;
            fixed4 _SecondColor;
            half _LerpSpeed;
            
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
            
			fixed4 frag (v2f i) : SV_Target
			{
                half speed = _LerpSpeed * 10.0;
                fixed4 col = lerp(_FirstColor, _SecondColor, (1 + sin(speed * _Time.z)) * 0.5);
                
                return fixed4(col.r, col.g, col.b, tex2D(_MaskTex, i.uv).r);
            
                //float2 scrollUV = i.uv;
                //scrollUV.x += _ScrollSpeedXY * _Time.x;

                //fixed4 color = tex2D(_MainTex, scrollUV);
                //return color;
			}
			ENDCG
		}
	}
}
