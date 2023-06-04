#pragma once

#include "./Input.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Shadow/ShadowSamplingTent.hlsl"

TEXTURE2D_SHADOW(_ShadowTexture);
SAMPLER_CMP(sampler_ShadowTexture);

CBUFFER_START(MainLightShadows)
float4x4 _WorldToShadowMatrix;
float3 _LightDirection;
float4 _ShadowParams; // x: Depth Bias, y: Normal Bias, z: Shadow Strength, w: Is Soft Shadow
float4 _ShadowTexture_TexelSize; // x: 1 / width, y: 1 / height, z: width, h: height
CBUFFER_END

float SampleShadowMap(float4 shadowCoord)
{
    return SAMPLE_TEXTURE2D_SHADOW(_ShadowTexture, sampler_ShadowTexture, shadowCoord.xyz);
}

float SampleShadowMapSoft(float4 shadowCoord)
{
    real tentWeights[9];
	real2 tentUVs[9];
    SampleShadow_ComputeSamples_Tent_5x5(_ShadowTexture_TexelSize, shadowCoord.xy, tentWeights, tentUVs);

    float attenuation = 0;

    for (int i = 0; i < 9; i++)
    {
        float4 coord = float4(tentUVs[i].xy, shadowCoord.z, 0);
        attenuation += tentWeights[i] * SampleShadowMap(coord);
    }

    return attenuation;
}

float4 TransformWorldToShadowCoord(float3 positionWS)
{
    return mul(_WorldToShadowMatrix, float4(positionWS, 1.0));
}

float ShadowAttenuation(float3 positionWS)
{
#ifdef _MAIN_LIGHT_SHADOWS
    float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
    
    #ifdef _SHADOWS_SOFT
        float attenuation = SampleShadowMapSoft(shadowCoord);
    #else
        float attenuation = SampleShadowMap(shadowCoord);
    #endif
    
    attenuation = LerpWhiteTo(attenuation, _ShadowParams.z);
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

