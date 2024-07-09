#pragma once

#include "./Input.hlsl"

float4 _ScreenParams; // x: width, y: height, z: 1 + 1.0 / width, w: 1 + 1.0 / height
float4 _ScaledScreenParams; // x: width, y: height, z: 1 + 1.0 / width, w: 1 + 1.0 / height

#ifdef UNITY_UV_STARTS_AT_TOP
    #define SCALE_BIAS float3(-1, 1, -1)
#else
    #define SCALE_BIAS float3(1, 0, 1)
#endif

void TransformScreenUV(inout float2 uv, float screenHeight)
{
#if UNITY_UV_STARTS_AT_TOP
    float3 bias = SCALE_BIAS;
    uv.y = screenHeight - (uv.y * bias.x + bias.y * screenHeight);
#endif
}

void TransformScreenUV(inout float2 uv)
{
#if UNITY_UV_STARTS_AT_TOP
    TransformScreenUV(uv, _ScaledScreenParams.y);
#endif
}

void TransformNormalizedScreenUV(inout float2 uv)
{
#if UNITY_UV_STARTS_AT_TOP
    TransformScreenUV(uv, 1.0);
#endif
}

float2 GetNormalizedScreenSpaceUV(float2 positionCS)
{
    float2 normalizedScreenSpaceUV = positionCS.xy * rcp(_ScaledScreenParams.xy);
    TransformNormalizedScreenUV(normalizedScreenSpaceUV);
    return normalizedScreenSpaceUV;
}

float2 GetNormalizedScreenSpaceUV(float4 positionCS)
{
    return GetNormalizedScreenSpaceUV(positionCS.xy);
}

float4 ComputeScreenPos(float4 positionCS)
{
    float4 ndc = positionCS * 0.5f;
    ndc.xy = float2(ndc.x, ndc.y * _ProjectionParams.x) + ndc.w;
    ndc.zw = positionCS.zw;

    return ndc;
}

float4 ComputeNDCPosition(float4 positionCS)
{
    float2 ndc = float2(positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - positionCS.y) : positionCS.y);
    ndc = ndc.xy / _ScaledScreenParams.xy;
    ndc.y = 1 - ndc.y;

    return float4(ndc.xy, 0, 1);
}

float Dither(float value, float2 posiitonNDC) {
    static float DITHER_THRESHOLDS[16] = {
        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
    };

    float2 uv = posiitonNDC.xy * _ScreenParams.xy;
    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;

    return value - DITHER_THRESHOLDS[index];
}
