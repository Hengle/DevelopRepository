#ifndef Common_INCLUDED
#define Common_INCLUDED

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

float rand(float2 value, float seed)
{
    return frac(sin(dot(value.xy, float2(12.9898, 78.233)) + floor(seed)) * 43758.5453);
}

    
#endif
