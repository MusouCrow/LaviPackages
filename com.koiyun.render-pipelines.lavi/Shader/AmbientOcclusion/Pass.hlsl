#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"
#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Depth.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "./Lib.hlsl"

float4 _LightColor;

struct Attributes
{
    uint vertexID : SV_VertexID;
    float4 positionOS : POSITION;
    float2 texcoord : TEXCOORD0;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
};

Varyings Vert(Attributes input)
{
    Varyings output;
    output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
    output.uv = GetFullScreenTriangleTexCoord(input.vertexID);

    return output;
}

float4 Frag(Varyings input) : SV_TARGET
{
    float depth = SampleDepth(input.uv, 3);
    depth = LinearEyeDepth(depth, _ZBufferParams);
    
    float3 positionVS = ReconstructViewPosition(input.uv, depth);
    float3 normalVS = ReconstructNormal(positionVS);
    
    float4x4 mtx = _AOMatrixVP;
    float3 transformA = float3(mtx._m00, mtx._m01, mtx._m02);
    float3 transformB = float3(mtx._m10, mtx._m11, mtx._m12);
    float rcpCount = rcp(SAMPLE_COUNT);
    float index = -1;
    float ao = 0;
    
    UNITY_UNROLL
    for (int i = 0; i < SAMPLE_COUNT; i++)
    {
        index += 1;

        float3 shift = PickSamplePoint(input.uv, i, index, rcpCount, normalVS);
        float3 shiftPosVS = float3(positionVS + shift);
        float2 shiftPosSS = float2(
            transformA.x * shiftPosVS.x + transformA.y * shiftPosVS.y + transformA.z * shiftPosVS.z,
            transformB.x * shiftPosVS.x + transformB.y * shiftPosVS.y + transformB.z * shiftPosVS.z
        );
        
        float zDist = -dot(UNITY_MATRIX_V[2].xyz, shiftPosVS);
        float2 uv = saturate((shiftPosSS * rcp(zDist) + 1) * 0.5);
        float depth2 = SampleDepth(uv, 3);
        float rawDpeth2 = depth2;
        depth2 = LinearEyeDepth(depth2, _ZBufferParams);

        float isInsideRadius = abs(zDist - depth2) < RADIUS ? 1.0 : 0.0;
        isInsideRadius *= rawDpeth2 > SKY_DEPTH ? 1.0 : 0.0;

        float3 positionVS2 = ReconstructViewPosition(uv, depth2);
        float3 dir = positionVS2 - positionVS;

        float dotVal = dot(dir, normalVS) - 0.004 * depth;
        float a1 = max(dotVal, 0);
        float a2 = dot(dir, dir) + 0.0001;
        ao += a1 * rcp(a2) * isInsideRadius;
    }

    return ao;
}
