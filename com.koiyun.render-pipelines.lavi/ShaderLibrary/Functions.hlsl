#pragma once

#include "./Shadow.hlsl"
#include "./View.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

void ShadowAttenuation_float(float3 PositionWS, out float Attenuation)
{
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    float scene = SAMPLE_SHADOW(_SceneShadowTexture, shadowCoord);
    float char = SAMPLE_SHADOW(_UnitShadowTexture, shadowCoord);
    
    Attenuation = lerp(_ShadowAttens.y, 1, scene) * lerp(_ShadowAttens.y, 1, char);
    Attenuation = max(_ShadowAttens.x, Attenuation);
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

void RGBToHSV_float(float3 RGB, out float Hue, out float Saturation, out float Lightness)
{
    float3 hsv = RgbToHsv(RGB);
    Hue = hsv.r;
    Saturation = hsv.g;
    Lightness = hsv.b;
}

void HSVToRGB_float(float Hue, float Saturation, float Lightness, out float3 RGB)
{
    float3 hsv = float3(Hue, Saturation, Lightness);
    RGB = HsvToRgb(hsv);
}

void SampleColorTable_float(UnityTexture2D ColorTableMap, float2 UV, float3 PositionWS, out float4 Color)
{
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    float scene = SAMPLE_SHADOW(_SceneShadowTexture, shadowCoord);
    float unit = SAMPLE_SHADOW(_UnitShadowTexture, shadowCoord);
    
    float pixel = ColorTableMap.texelSize.x * 32;
    float index = floor(UV.x / pixel);
    float base = floor(index / 3) * 3;
    float rate = 1 - unit;
    float2 uv = UV;
    uv.x = (base + 1) * pixel;
    
    float4 bright = ColorTableMap.Sample(sampler_PointClamp, UV);
    float4 dark = ColorTableMap.Sample(sampler_PointClamp, uv);

    float attenuationBright = lerp(_ShadowAttens.w, 1, scene);
    float attenuationDark = lerp(_ShadowAttens.z, 1, scene);

    Color = lerp(bright * attenuationBright, dark * attenuationDark, rate);
}

void Gradient_float(float Value, float Power, out float Gradient)
{
    float v = pow(Value, lerp(0, 2, Power));
    Gradient = saturate(v);
}

void Metallic_float(float3 PosiitonWS, float3 NormalWS, float Rate, float Gradient, out float Metallic, out float Glow)
{
    float3 viewDir = GetWorldSpaceNormalizeViewDir(PosiitonWS);
    float3 normalDir = normalize(NormalWS - Gradient);
    float3 reflectDir = reflect(-_LightDirection, normalDir);
    float v = saturate(dot(viewDir, reflectDir) + Gradient);
    v = lerp(-0.5, 1, v);

    Metallic = v * Rate;
    Glow = v * 0.01 * Rate;
}

void TexIndexToUV_float(float Index, float2 SheetSize, out float2 UV)
{
    float hang = Index % SheetSize.x;
    float lie = floor(Index / SheetSize.x);
    
    UV = float2(hang, (lie + 1)) / SheetSize;
    UV.y = 1 - UV.y;
}
