#pragma once

#include "./Shadow.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"

TEXTURE2D(_ColorTableTexture);
SAMPLER(sampler_PointClamp);
SAMPLER(sampler_LinearClamp);
float4 _ColorTableTexture_TexelSize;

void ShadowAttenuation_float(float3 PositionWS, out float Attenuation)
{
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    float scene = SAMPLE_SHADOW(_SceneShadowTexture, shadowCoord);
    float char = SAMPLE_SHADOW(_UnitShadowTexture, shadowCoord);

    Attenuation = lerp(0.3, 1, scene) * lerp(0.3, 1, char);
    Attenuation = max(0.2, Attenuation);
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

void SampleColorTable_float(float2 UV, float3 PositionWS, out float4 Color)
{
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    float scene = SAMPLE_SHADOW(_SceneShadowTexture, shadowCoord);
    float unit = SAMPLE_SHADOW(_UnitShadowTexture, shadowCoord);
    
    float pixel = _ColorTableTexture_TexelSize.x * 32;
    float index = floor(UV.x / pixel);
    float base = floor(index / 3) * 3;
    float rate = 1 - unit;
    float2 uv = UV;
    uv.x = (base + 1) * pixel;

    float4 bright = SAMPLE_TEXTURE2D(_ColorTableTexture, sampler_PointClamp, UV);
    float4 dark = SAMPLE_TEXTURE2D(_ColorTableTexture, sampler_PointClamp, uv);

    float attenuationBright = lerp(0.3, 1, scene);
    float attenuationDark = lerp(0.5, 1, scene);

    Color = lerp(bright * attenuationBright, dark * attenuationDark, rate);
}
/*
void Sobel_float(UnityTexture2D ColorMap, float2 UV, out float Edge)
{
    const half2 DX[9] = {
        {-1, -1}, {0, -1}, {1, -1},
        {-1, 0}, {0, 0}, {1, 0},
        {-1, 1}, {0, 1}, {1, 1}
    };

    const half GX[9] = {
        -1, 0, 1,
        -2, 0, 2,
        -1, 0, 1
    };

    const half GY[9] = {
        -1, -2, -1,
        0, 0, 0,
        1, 2, 1
    };

    float edgeX = 0;
    float edgeY = 0;

    for (int i = 0; i < 9; i++) {
        float2 uv = UV + DX[i] * ColorMap.texelSize.xy;
        float bright = ColorMap.Sample(sampler_LinearClamp, uv);
        edgeX += bright * GX[i];
        edgeY += bright * GY[i];
    }

    Edge = 1 - abs(edgeX) - abs(edgeY);
    Edge = saturate(Edge);
}
*/

void Sobel_float(UnityTexture2D ColorMap, float2 UV, out float Edge)
{
    const half2 DX[9] = {
        {0, 0},
        {-1, 0}, {-2, 0},
        {1, 0}, {2, 0},
        {0, -1}, {0, -2},
        {0, 1}, {0, 2},
    };

    const half SO[9] = {
        8,
        -1, -1,
        -1, -1,
        -1, -1,
        -1, -1
    };

    Edge = 0;

    for (int i = 0; i < 9; i++) {
        float2 uv = UV + DX[i] * ColorMap.texelSize.xy;
        float bright = ColorMap.Sample(sampler_LinearClamp, uv);
        Edge += bright * SO[i];
    }
    
    Edge = Edge > 0.2 ? 1 : 0;
}