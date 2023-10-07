#pragma once

#include "./Include.hlsl"

float4 Frag(Varyings input) : SV_Target
{
    return SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, input.uv);
}