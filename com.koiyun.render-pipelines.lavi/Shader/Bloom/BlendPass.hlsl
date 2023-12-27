#pragma once

#include "./Include.hlsl"

TEXTURE2D(_TempColorTexture);
TEXTURE2D(_BloomBlurTexture);

float4 Frag(Varyings input) : SV_Target
{
    float4 color = SAMPLE_TEXTURE2D_LOD(_TempColorTexture, sampler_LinearClamp, input.uv, 0);
    float4 blur = SAMPLE_TEXTURE2D_LOD(_BloomBlurTexture, sampler_LinearClamp, input.uv, 0);
    
    color.rgb += blur.rgb;
    
    return color;
}
