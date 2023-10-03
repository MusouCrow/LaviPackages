#pragma once

#include "./Shadow.hlsl"

void ShadowAttenuation_float(float3 PositionWS, out float Attenuation)
{
    Attenuation = ShadowAttenuation(PositionWS);
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
