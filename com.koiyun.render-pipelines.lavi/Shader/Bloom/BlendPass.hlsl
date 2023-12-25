#pragma once

#include "./Include.hlsl"

TEXTURE2D(_BloomTexture);
TEXTURE2D(_BloomBlurTexture);

float4 Frag(Varyings input) : SV_Target
{
    float4 color = SAMPLE_TEXTURE2D(_BloomTexture, sampler_LinearClamp, input.uv);
    float4 blur = SAMPLE_TEXTURE2D(_BloomBlurTexture, sampler_LinearClamp, input.uv);
    
    color.rgb += blur.rgb;
    
    return color;
}
