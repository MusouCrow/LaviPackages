#pragma once

#include "./Shadow.hlsl"

void ShadowAttenuation_float(float3 PositionWS, out float Attenuation)
{
    Attenuation = ShadowAttenuation(PositionWS);
}
