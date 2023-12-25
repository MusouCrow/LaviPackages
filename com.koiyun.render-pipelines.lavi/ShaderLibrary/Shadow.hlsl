#pragma once

#include "./Input.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Shadow/ShadowSamplingTent.hlsl"

TEXTURE2D_SHADOW(_SceneShadowTexture);
TEXTURE2D_SHADOW(_UnitShadowTexture);
SAMPLER_CMP(sampler_SceneShadowTexture);
SAMPLER_CMP(sampler_UnitShadowTexture);

CBUFFER_START(MainLightShadows)
float4x4 _WorldToShadowMatrix;
float3 _LightDirection;
float4 _ShadowParams; // x: Depth Bias, y: Normal Bias, z: Shadow Strength, w: Is Soft Shadow
float4 _SceneShadowTexture_TexelSize;
float4 _UnitShadowTexture_TexelSize;
CBUFFER_END

#define SAMPLE_SHADOW(name, shadowCoord) ShadowAttenuation(TEXTURE2D_SHADOW_ARGS(name, sampler##name), name##_TexelSize, shadowCoord)

float SampleShadowMap(TEXTURE2D_SHADOW_PARAM(shadowMap, sampler_shadowMap), float4 shadowCoord)
{
    return SAMPLE_TEXTURE2D_SHADOW(shadowMap, sampler_shadowMap, shadowCoord.xyz);
}

float SampleShadowMapSoft(TEXTURE2D_SHADOW_PARAM(shadowMap, sampler_shadowMap), float4 texelSize, float4 shadowCoord)
{
    real tentWeights[9];
	real2 tentUVs[9];
    SampleShadow_ComputeSamples_Tent_5x5(texelSize, shadowCoord.xy, tentWeights, tentUVs);

    float attenuation = 0;

    for (int i = 0; i < 9; i++)
    {
        float4 coord = float4(tentUVs[i].xy, shadowCoord.z, 0);
        attenuation += tentWeights[i] * SampleShadowMap(TEXTURE2D_SHADOW_ARGS(shadowMap, sampler_shadowMap), coord);
    }

    return attenuation;
}

float4 TransformWorldToShadowCoord(float3 positionWS)
{
    return mul(_WorldToShadowMatrix, float4(positionWS, 1.0));
}

float ShadowAttenuation(TEXTURE2D_SHADOW_PARAM(shadowMap, sampler_shadowMap), float4 texelSize, float4 shadowCoord)
{

#ifdef _MAIN_LIGHT_SHADOWS
    float attenuation = SampleShadowMapSoft(TEXTURE2D_SHADOW_ARGS(shadowMap, sampler_shadowMap), texelSize, shadowCoord);
    
    attenuation = LerpWhiteTo(attenuation, 1);
    attenuation = step(0.7, attenuation);
#else
    float attenuation = 1;
#endif

    return attenuation;
}

float3 ApplyShadowBias(float3 positionWS, float3 normalWS)
{
    float invNdotL = 1.0 - saturate(dot(_LightDirection, normalWS));
    float scale = invNdotL * _ShadowParams.y;
    
    positionWS = _LightDirection * _ShadowParams.xxx + positionWS;
    positionWS = normalWS * scale.xxx + positionWS;
    
    return positionWS;
}
