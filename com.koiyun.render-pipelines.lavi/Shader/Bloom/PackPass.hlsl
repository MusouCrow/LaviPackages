#pragma once

#include "./Include.hlsl"

TEXTURE2D(_ColorTexture);
TEXTURE2D(_RawParamTexture);

float4 Frag(Varyings input) : SV_Target
{
    float4 color = SAMPLE_TEXTURE2D_LOD(_ColorTexture, sampler_LinearClamp, input.uv, 0);
    float4 param = SAMPLE_TEXTURE2D_LOD(_RawParamTexture, sampler_LinearClamp, input.uv, 0);

    color.rgb *= param.r * 10;
    color.a = param.g;
    
    return color;
}
