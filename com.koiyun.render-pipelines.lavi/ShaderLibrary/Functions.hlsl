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
