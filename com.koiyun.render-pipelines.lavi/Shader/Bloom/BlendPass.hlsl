#pragma once

#include "./Include.hlsl"

float4 Frag(Varyings input) : SV_Target
{
    float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, input.uv);
    float4 blur = SAMPLE_TEXTURE2D(_BloomBlurTexture, sampler_LinearClamp, input.uv);
    float anti = 1 - SAMPLE_TEXTURE2D(_CameraGlowTexture, sampler_LinearClamp, input.uv).g;
    half brightness = Max3(color.r, color.g, color.b);
    
    color.rgb += blur.rgb * anti;
    color.rgb = lerp(color.rgb, blur.rgb, saturate(blur.a * brightness * brightness * anti));
    
    return color;
}
