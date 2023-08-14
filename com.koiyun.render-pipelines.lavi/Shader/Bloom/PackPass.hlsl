#pragma once

#include "./Include.hlsl"

float4 Frag(Varyings input) : SV_Target
{
    float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, input.uv);
    float glow = SAMPLE_TEXTURE2D(_CameraGlowTexture, sampler_LinearClamp, input.uv).r;
    glow = saturate(glow) * 10;
    
    color.a = glow;
    color.rgb *= glow;

    return color;
}