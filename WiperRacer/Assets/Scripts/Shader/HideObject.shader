Shader "Custom/HideObject"
{
	Properties
	{
	   _Mask("Mask", Int) = 1
	}
	SubShader{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry-1"}
		Pass{

		ColorMask 0 //描写しない
		//ZWrite Off
		Stencil
		{
			Ref[_Mask]
			Comp Always
			Pass Replace
		}
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		//頂点座標をそのまま返す
		float4 vert(float4 v:POSITION) :SV_POSITION{
		return UnityObjectToClipPos(v);
		}

		//最小限の色だけを返す
		fixed4 frag() : COLOR{
			return fixed4(0,0,0,0);
		}
		ENDCG
		}
	}
    FallBack "Diffuse"
}
