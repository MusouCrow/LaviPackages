#pragma once

#include "./Shadow.hlsl"
#include "./View.hlsl"
#include "./Fog.hlsl"
#include "./Render.hlsl"
#include "./ShaderGraphFunctions.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

float4 _ColorTexture_TexelSize;
float _LightIntensty;
float4 _LightColor;
float _GlobalDeepRate;

void ShadowAttenuation_float(float3 PositionWS, bool HardShadow, bool NoChar, out float Attenuation)
{
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    float scene = SAMPLE_SHADOW(_SceneShadowTexture, shadowCoord, false);
    float char = NoChar ? 1 : SAMPLE_SHADOW(_UnitShadowTexture, shadowCoord, HardShadow);
    
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

void SampleColorTable_float(UnityTexture2D ColorTableMap, float2 UV, float3 PositionWS, float AntiShadow, bool HardShadow, out float4 Color)
{
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    float scene = SAMPLE_SHADOW(_SceneShadowTexture, shadowCoord, false);
    float unit = SAMPLE_SHADOW(_UnitShadowTexture, shadowCoord, HardShadow);
    
    float pixel = ColorTableMap.texelSize.x * 32;
    float index = floor(UV.x / pixel);
    float base = floor(index / 3) * 3;
    float sceneRate = lerp(scene, 1, AntiShadow);
    float unitRate = lerp(1 - unit, 0, AntiShadow);
    float2 uv = UV;
    uv.x = (base + 1) * pixel;
    
    float4 bright = ColorTableMap.Sample(sampler_PointClamp, UV);
    float4 dark = ColorTableMap.Sample(sampler_PointClamp, uv);

    float attenuationBright = lerp(_ShadowAttens.w, 1, sceneRate);
    float attenuationDark = lerp(_ShadowAttens.z, 1, sceneRate);

    Color = lerp(bright * attenuationBright, dark * attenuationDark, unitRate);
}

void Gradient_float(float Value, float Power, out float Gradient)
{
    float v = pow(Value, lerp(0, 2, Power));
    Gradient = saturate(v);
}

void Metallic_float(float3 PosiitonWS, float3 NormalWS, float Rate, out float Metallic, out float Glow)
{
    float3 viewDir = GetWorldSpaceNormalizeViewDir(PosiitonWS);
    float3 normalDir = normalize(NormalWS);
    float3 reflectDir = reflect(-_LightDirection, normalDir);
    float v = saturate(dot(viewDir, reflectDir));

    Metallic = lerp(-0.75, 1, v) * Rate;
    Glow = lerp(0, 0.02, v) * Rate;
}

void TexIndexToUV_float(float Index, float2 SheetSize, out float2 UV)
{
    float hang = Index % SheetSize.x;
    float lie = floor(Index / SheetSize.x);
    
    UV = float2(hang, (lie + 1)) / SheetSize;
    UV.y = 1 - UV.y;
}

void GetTime_float(out float Time)
{
    Time = _Time;
}

void RadialBlur_float(float2 UV, float2 Center, float Rate, out float3 RGB)
{
    int step = lerp(1, 30, saturate(Rate));
    float2 dir = (Center - UV) * 100 * _ColorTexture_TexelSize.xy;
    float2 uv = UV;
    RGB = 0;
    
    [unroll(30)]
    for (int i = 0; i < step; i++)
    {
        RGB += shadergraph_SampleSceneColor_Lavi(uv + dir * i);
    }

    RGB = RGB / step;
    // RGB = 1 - shadergraph_SampleSceneColor_Lavi(UV);
}

void Fog_float(float3 Color, float3 PosiitonWS, out float3 RGB, out float Rate)
{
    float4 ret = HighFog(Color, PosiitonWS);
    RGB = ret.rgb;
    Rate = ret.a;
}

void GetLightIntensty_float(out float Out)
{
    Out = _LightIntensty;
}

void GetLightColor_float(out float4 Out, out float Rate)
{
    Out = _LightColor;
    Rate = _LightColor.a;
}

void ParallaxMapping_float(float Height, float Rate, float2 UV, float3 ViewDirTS, out float2 Out)
{
    float rate = lerp(0, 0.08, Rate);
    Out = UV + ParallaxOffset1Step(Height, rate, ViewDirTS);
}

void GetGlobalDeepRate_float(out float Deep)
{
    Deep = _GlobalDeepRate;
}

void Contrast_float(float4 Color, float Contrast, out float4 Out)
{
    float3 rgb = Color.rgb;
    rgb = lerp(0.5, rgb, Contrast);
    
    Out = float4(rgb, Color.a);
}
