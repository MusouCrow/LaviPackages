#pragma once

#include "./Shadow.hlsl"

SAMPLER(sampler_PointClamp);

void ShadowAttenuation_float(float3 PositionWS, out float Attenuation)
{
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    float scene = ShadowAttenuation(TEXTURE2D_SHADOW_ARGS(_ShadowTexture, sampler_ShadowTexture), shadowCoord);
    float char = ShadowAttenuation(TEXTURE2D_SHADOW_ARGS(_CharShadowTexture, sampler_CharShadowTexture), shadowCoord);

    Attenuation = lerp(0.3, 1, scene) * lerp(0.3, 1, char);
    Attenuation = max(0.2, Attenuation);
}

void ShadowAttenuationScene_float(float3 PositionWS, out float Attenuation)
{
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    float scene = ShadowAttenuation(TEXTURE2D_SHADOW_ARGS(_ShadowTexture, sampler_ShadowTexture), shadowCoord);

    Attenuation = lerp(0.3, 1, scene);
}

void ShadowAttenuationChar_float(float3 PositionWS, out float Attenuation)
{
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    float char = ShadowAttenuation(TEXTURE2D_SHADOW_ARGS(_CharShadowTexture, sampler_CharShadowTexture), shadowCoord);

    Attenuation = 1 - char;
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

void SampleColorTable_float(UnityTexture2D ColorMap, float2 UV, float3 PositionWS, out float4 Color)
{
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    float scene = ShadowAttenuation(TEXTURE2D_SHADOW_ARGS(_ShadowTexture, sampler_ShadowTexture), shadowCoord);
    float char = ShadowAttenuation(TEXTURE2D_SHADOW_ARGS(_CharShadowTexture, sampler_CharShadowTexture), shadowCoord);

    float unit = ColorMap.texelSize.x * 32;
    float index = floor(UV.x / unit);
    float base = floor(index / 3) * 3;
    float rate = 1 - char;
    float2 uv = UV;
    uv.x = (base + 1) * unit;

    float4 bright = ColorMap.Sample(sampler_PointClamp, UV);
    float4 dark = ColorMap.Sample(sampler_PointClamp, uv);

    float attenuationBright = lerp(0.3, 1, scene);
    float attenuationDark = lerp(0.5, 1, scene);

    Color = lerp(bright * attenuationBright, dark * attenuationDark, rate);
}
