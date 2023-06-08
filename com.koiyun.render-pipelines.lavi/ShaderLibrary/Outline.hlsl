#pragma once

#include "./Core.hlsl"

float4 GetOutlinePositionHClip(float3 positionOS, float3 normalOS, float scale)
{
    float4 positionVS = mul(UNITY_MATRIX_MV, float4(positionOS, 1.0));
    float3 normalVS = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, normalOS));
    
    positionVS.xyz += normalVS * scale;
    
    return mul(UNITY_MATRIX_P, positionVS);
}