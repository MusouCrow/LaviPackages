#pragma once

#include "./Include.hlsl"

float4 Frag(Varyings input) : SV_Target
{
    float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, input.uv);
    float4 blur = SAMPLE_TEXTURE2D(_BloomBlurTexture, sampler_LinearClamp, input.uv);
    half brightness = Max3(color.r, color.g, color.b) * 0.5;

    color.rgb += blur.rgb;
    color.rgb = lerp(color.rgb, blur.rgb, saturate(blur.a * (1 - brightness)));
    
    return color;
}
