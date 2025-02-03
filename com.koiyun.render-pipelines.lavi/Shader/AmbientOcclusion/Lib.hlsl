#pragma once

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"
#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Screen.hlsl"

#define SKY_DEPTH 0.00001

float4x4 _AOMatrixVP;
float4 _AOViewExtentX;
float4 _AOViewExtentY;
float4 _AOViewExtentZ;

static float SSAORandomUV[40] =
{
    0.00000000,  // 00
    0.33984375,  // 01
    0.75390625,  // 02
    0.56640625,  // 03
    0.98437500,  // 04
    0.07421875,  // 05
    0.23828125,  // 06
    0.64062500,  // 07
    0.35937500,  // 08
    0.50781250,  // 09
    0.38281250,  // 10
    0.98437500,  // 11
    0.17578125,  // 12
    0.53906250,  // 13
    0.28515625,  // 14
    0.23137260,  // 15
    0.45882360,  // 16
    0.54117650,  // 17
    0.12941180,  // 18
    0.64313730,  // 19

    0.92968750,  // 20
    0.76171875,  // 21
    0.13333330,  // 22
    0.01562500,  // 23
    0.00000000,  // 24
    0.10546875,  // 25
    0.64062500,  // 26
    0.74609375,  // 27
    0.67968750,  // 28
    0.35156250,  // 29
    0.49218750,  // 30
    0.12500000,  // 31
    0.26562500,  // 32
    0.62500000,  // 33
    0.44531250,  // 34
    0.17647060,  // 35
    0.44705890,  // 36
    0.93333340,  // 37
    0.87058830,  // 38
    0.56862750,  // 39
};

float3 ReconstructViewPosition(float2 uv, float linearDepth)
{
    uv.y = 1.0 - uv.y;

    float zScale = linearDepth * (1.0 / _ProjectionParams.y); // divide by near plane
    float3 positionVS = _AOViewExtentX.xyz + _AOViewExtentY.xyz * uv.x + _AOViewExtentZ.xyz * uv.y;
    positionVS *= zScale;

    return positionVS;
}

float3 ReconstructNormal(float3 positionVS)
{
    return float3(normalize(cross(ddy(positionVS), ddx(positionVS))));
}

float GetRandomVal(float u, float sampleIndex)
{
    return SSAORandomUV[u * 20 + sampleIndex];
}

float3 PickSamplePoint(float2 uv, int index, float index2, float rcpCount, float3 normalVS, float radius)
{
    float2 positionSS = uv * _ScreenParams.xy;
    float noise = InterleavedGradientNoise(positionSS, index);
    float u = frac(GetRandomVal(0, index) + noise) * 2 - 1;
    float theta = (GetRandomVal(1, index) + noise) * 6.28318530717958647693;
    float u2 = sqrt(1 - u * u);

    float3 v = float3(u2 * cos(theta), u2 * sin(theta), u);
    v *= sqrt((index2 + 1.0) * rcpCount);
    v = faceforward(v, -normalVS, v);
    v *= radius;
    
    return v;
}

float AOProcess(float sampleCount, float radius, float2 uv, float3 normalVS, float3 positionVS, float depth)
{
    float4x4 mtx = _AOMatrixVP;
    float3 transformA = float3(mtx._m00, mtx._m01, mtx._m02);
    float3 transformB = float3(mtx._m10, mtx._m11, mtx._m12);
    float rcpCount = rcp(sampleCount);
    float index = -1;
    float ao = 0;

    UNITY_UNROLL
    for (int i = 0; i < sampleCount; i++)
    {
        index += 1;

        float3 shift = PickSamplePoint(uv, i, index, rcpCount, normalVS, radius);
        float3 shiftPosVS = float3(positionVS + shift);
        float2 shiftPosSS = float2(
            transformA.x * shiftPosVS.x + transformA.y * shiftPosVS.y + transformA.z * shiftPosVS.z,
            transformB.x * shiftPosVS.x + transformB.y * shiftPosVS.y + transformB.z * shiftPosVS.z
        );
        
        float zDist = -dot(UNITY_MATRIX_V[2].xyz, shiftPosVS);
        float2 uv2 = saturate((shiftPosSS * rcp(zDist) + 1) * 0.5);
        float depth2 = SampleDepth(uv2, 3);
        float rawDpeth2 = depth2;
        depth2 = LinearEyeDepth(depth2, _ZBufferParams);

        float isInsideRadius = abs(zDist - depth2) < radius ? 1.0 : 0.0;
        isInsideRadius *= rawDpeth2 > SKY_DEPTH ? 1.0 : 0.0;

        float3 positionVS2 = ReconstructViewPosition(uv2, depth2);
        float3 dir = positionVS2 - positionVS;

        float dotVal = dot(dir, normalVS) - 0.004 * depth;
        float a1 = max(dotVal, 0);
        float a2 = dot(dir, dir) + 0.0001;
        ao += a1 * rcp(a2) * isInsideRadius;
    }

    return ao;
}
