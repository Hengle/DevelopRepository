Shader "Custom/CrazyCharacterShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_DamageColor("Damage Color", Color) = (1,1,1,1)

		_MainTex("Main (RGB)", 2D) = "white" {}
		_DamageTex("Damage (RGB)", 2D) = "white" {}
		_DissolveTex("Dissolve (RGB)", 2D) = "white" {}
		_MaskTex("Mask", 2D) = "white"{}
		_BumpTex("Bump", 2D) = "White"{}

		_DamageThreshold("DamageThreshold", Range(0, 1)) = 0
		_DissolveThreshold("DissolveThreshold", Range(0, 1)) = 0

		[HDR]_GlowColor("Color", Color) = (1, 1, 1, 1)
		_GlowRange("Range", Range(0, .5)) = 0.1
		_GlowFalloff("Falloff", Range(0, 1)) = 0.1

		_Hue("Hue", Float) = 0
		_Sat("Saturation", Float) = 1
		_Val("Value", Float) = 1
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_SilhouetteColor("Silhouette Color", Color) = (0, 0, 0, 1)
	}

		SubShader{
			Tags { "Queue" = "Geometry+1" "RenderType" = "Opaque" }

			/*Pass {
			// Don't write to the depth buffer for the silhouette pass
			ZWrite Off
			ZTest Always

			// First Pass: Silhouette
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float4 _SilhouetteColor;

			struct vertInput {
				float4 vertex:POSITION;
				float3 normal:NORMAL;
			};

			struct fragInput {
				float4 pos:SV_POSITION;
			};

			fragInput vert(vertInput i) {
				fragInput o;
				o.pos = UnityObjectToClipPos(i.vertex);
				return o;
			}

			float4 frag(fragInput i) : COLOR {
				return _SilhouetteColor;
			}

			ENDCG
		}*/


		CGPROGRAM

		#pragma surface surf Standard vertex:vert fullforwardshadows
		#pragma target 3.0
		fixed4 _Color;
		fixed4 _DamageColor;

		sampler2D _MainTex;
		sampler2D _DamageTex;
		sampler2D _DissolveTex;
		sampler2D _MaskTex;
		sampler2D _BumpTex;

		float _DamageThreshold;
		float _DissolveThreshold;

		fixed3 _GlowColor;
		half _GlowRange;
		half _GlowFalloff;
		half _Hue, _Sat, _Val;
		half _Glossiness;
		half _Metallic;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		// RGB->HSV変換
		float3 rgb2hsv(float3 rgb)
		{
			float3 hsv;

			// RGBの三つの値で最大のもの
			float maxValue = max(rgb.r, max(rgb.g, rgb.b));
			// RGBの三つの値で最小のもの
			float minValue = min(rgb.r, min(rgb.g, rgb.b));
			// 最大値と最小値の差
			float delta = maxValue - minValue;

			// V（明度）
			// 一番強い色をV値にする
			hsv.z = maxValue;

			// S（彩度）
			// 最大値と最小値の差を正規化して求める
			if (maxValue != 0.0) {
				hsv.y = delta / maxValue;
			}
			else {
				hsv.y = 0.0;
			}

			// H（色相）
			// RGBのうち最大値と最小値の差から求める
			if (hsv.y > 0.0) {
				if (rgb.r == maxValue) {
					hsv.x = (rgb.g - rgb.b) / delta;
				}
				else if (rgb.g == maxValue) {
					hsv.x = 2 + (rgb.b - rgb.r) / delta;
				}
				else {
					hsv.x = 4 + (rgb.r - rgb.g) / delta;
				}
				hsv.x /= 6.0;
				if (hsv.x < 0)
				{
					hsv.x += 1.0;
				}
			}

			return hsv;
		}

		// HSV->RGB変換
		float3 hsv2rgb(float3 hsv)
		{
			float3 rgb;

			if (hsv.y == 0) {
				// S（彩度）が0と等しいならば無色もしくは灰色
				rgb.r = rgb.g = rgb.b = hsv.z;
			}
			else {
				// 色環のH（色相）の位置とS（彩度）、V（明度）からRGB値を算出する
				hsv.x *= 6.0;
				float i = floor(hsv.x);
				float f = hsv.x - i;
				float aa = hsv.z * (1 - hsv.y);
				float bb = hsv.z * (1 - (hsv.y * f));
				float cc = hsv.z * (1 - (hsv.y * (1 - f)));
				if (i < 1) {
					rgb.r = hsv.z;
					rgb.g = cc;
					rgb.b = aa;
				}
				else if (i < 2) {
					rgb.r = bb;
					rgb.g = hsv.z;
					rgb.b = aa;
				}
				else if (i < 3) {
					rgb.r = aa;
					rgb.g = hsv.z;
					rgb.b = cc;
				}
				else if (i < 4) {
					rgb.r = aa;
					rgb.g = bb;
					rgb.b = hsv.z;
				}
				else if (i < 5) {
					rgb.r = cc;
					rgb.g = aa;
					rgb.b = hsv.z;
				}
				else {
					rgb.r = hsv.z;
					rgb.g = aa;
					rgb.b = bb;
				}
			}
			return rgb;
		}

		float3 shift_col(float3 rgb, half3 shift)
		{
			// RGB->HSV変換
			float3 hsv = rgb2hsv(rgb);

			// HSV操作
			hsv.x += shift.x;
			if (1.0 <= hsv.x)
			{
				hsv.x -= 1.0;
			}
			hsv.y *= shift.y;
			hsv.z *= shift.z;

			// HSV->RGB変換
			return hsv2rgb(hsv);
		}

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			v.vertex.xyz = float3(v.vertex.x, v.vertex.y, v.vertex.z);

			//float dist = distance(fixed3(1, 1, 1), o.worldPos) * _DamageThreshold;
			//v.vertex.xyz = float3(v.vertex.x * dist, v.vertex.y * dist, v.vertex.z * dist);
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c1 = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 c2 = tex2D(_DamageTex, IN.uv_MainTex) * _DamageColor;
			fixed4 c3 = tex2D(_DissolveTex, IN.uv_MainTex);
			fixed4 mask = tex2D(_MaskTex, IN.uv_MainTex);

			float dissolve = c3.r;
			dissolve = dissolve * 0.999;
			float isVisible = dissolve - _DissolveThreshold;
			clip(isVisible);

			float isGlowing = smoothstep(_GlowRange + _GlowFalloff, _GlowRange, isVisible);
			float3 glow = isGlowing * _GlowColor;

			if (_DissolveThreshold > 0.001) {
				c1 = fixed4(c1.r + glow.x, c1.g + glow.y, c1.b + glow.z, c1.a);
			}

			c1.rgb = lerp(c1, c2, mask.r * _DamageThreshold);

			half3 shift = half3(_Hue, _Sat, _Val);
			fixed4 shiftColor = fixed4(shift_col(c1.rgb, shift), c1.a);

			o.Albedo = shiftColor.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = shiftColor.a;
			//o.Normal = UnpackNormal(tex2D(_BumpTex, IN.uv_MainTex));
		}

		ENDCG
		}

			FallBack "Diffuse"
}