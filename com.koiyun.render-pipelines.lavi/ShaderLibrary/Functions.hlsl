#pragma once

#include "./Shadow.hlsl"

SAMPLER(sampler_LinearClamp);

void ShadowAttenuation_float(float3 PositionWS, out float Attenuation)
{
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    Attenuation = ShadowAttenuation(shadowCoord);
    Attenuation = max(Attenuation - CharShadowAttenuation(shadowCoord, _ShadowParams.z), 0.2);
}

void ShadowAttenuationScene_float(float3 PositionWS, out float Attenuation)
{
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    Attenuation = ShadowAttenuation(shadowCoord);
}

void ShadowAttenuationChar_float(float3 PositionWS, out float Attenuation)
{
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    Attenuation = CharShadowAttenuation(shadowCoord, 1);
}

void LerpColor_float(float4 BaseColor, float4 LerpColor, out float4 Color)
{
    Color.rgb = lerp(BaseColor.rgb, LerpColor.rgb, LerpColor.a);
    Color.a = BaseColor.a;
}

void Rim_float(float3 ViewDir, float3 NormalWS, float2 Range, out float Rim)
{
    float VDotN = dot(ViewDir, NormalWS);
    VDotN = 1 - saturate(VDotN);
    Rim = smoothstep(Range[0], Range[1], VDotN);
}

void HSV_float(float3 RGB, out float Hue, out float Saturation, out float Lightness)
{
    float3 hsv = RgbToHsv(RGB);
    Hue = hsv.r;
    Saturation = hsv.g;
    Lightness = hsv.b;
}

void SampleColorTable_float(UnityTexture2D ColorMap, float2 UV, float Attenuation, out float4 Color)
{
    float2 uv = UV;
    uv.x += ColorMap.texelSize.x * 32;
    
    float4 bright = ColorMap.Sample(sampler_LinearClamp, UV);
    float4 dark = ColorMap.Sample(sampler_LinearClamp, uv);
    Color = lerp(bright, dark, Attenuation);
}
