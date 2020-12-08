Shader "Custom/MonoTone"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _Threshold("Threshold", Range(0,1)) = 0.0
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            half _Threshold;

            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                float grayScale = color.r * .3 + color.g * .6 + color.b * .1;
                grayScale *= _Threshold;
                
                fixed3 lColor = lerp(color.rgb, grayScale, _Threshold);
                return fixed4(lColor.r, lColor.g, lColor.b, color.a);
            }
            ENDCG
        }
    }
}
