Shader "Metaball/MetaballParticle" {
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)			//カラー
		_Scale("Scale", Range(0,0.05)) = 0.01		//スケール
		_Cutoff("Cutoff", Range(0,05)) = 0.01		//アルファ値が闘値を超えた部分のレンダリングしない
	}

		//Σ内の計算
		//1/((x-xi)² + (y-yi)²)
		SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"				//Projectorの投影を切る
			"PreviewType" = "Plane"					//Preview画面の3D形状
		}

		Cull Off									//カリング
		Lighting Off								//ライティング
		ZWrite Off									//オブジェクトのピクセルをデプスバッファ書き込み
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog				// フォグ用のバリアントを生成

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			fixed4 _Color;
			fixed _Scale;
			fixed _Cutoff;

			v2f vert(appdata v)
			{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);		//頂点
					o.texcoord = v.texcoord;						//UV
					o.color = v.color * _Color;						//色
					return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				//MetaBall数式で計算したピクセル情報を返す
				fixed2 uv = i.texcoord - 0.5;
				fixed a = 1 / (uv.x * uv.x + uv.y * uv.y);
				a *= _Scale;
				fixed4 color = i.color * a;
				clip(color.a - _Cutoff);

				return color;
			}
		 ENDCG
		 }
	}
}