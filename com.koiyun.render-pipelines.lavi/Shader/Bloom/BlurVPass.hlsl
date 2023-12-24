#pragma once

#include "./Include.hlsl"

TEXTURE2D(_BloomBlurMap);
float4 _BloomBlurMap_TexelSize;

float4 Frag(Varyings input) : SV_Target
{
    float2 size = _BloomBlurMap_TexelSize;
    float2 uv = input.uv;
    
    float u = 1;
    float4 c0 = SAMPLE_TEXTURE2D(_BloomBlurMap, sampler_LinearClamp, uv + size * float2(u, 0));
    float4 c1 = SAMPLE_TEXTURE2D(_BloomBlurMap, sampler_LinearClamp, uv + size * float2(-u, 0));
    float4 c2 = SAMPLE_TEXTURE2D(_BloomBlurMap, sampler_LinearClamp, uv + size * float2(0, u));
    float4 c3 = SAMPLE_TEXTURE2D(_BloomBlurMap, sampler_LinearClamp, uv + size * float2(0, -u));
    float4 c4 = SAMPLE_TEXTURE2D(_BloomBlurMap, sampler_LinearClamp, uv + size * float2(u, u));
    float4 c5 = SAMPLE_TEXTURE2D(_BloomBlurMap, sampler_LinearClamp, uv + size * float2(u, -u));
    float4 c6 = SAMPLE_TEXTURE2D(_BloomBlurMap, sampler_LinearClamp, uv + size * float2(-u, u));
    float4 c7 = SAMPLE_TEXTURE2D(_BloomBlurMap, sampler_LinearClamp, uv + size * float2(-u, 0));
    float4 c8 = SAMPLE_TEXTURE2D(_BloomBlurMap, sampler_LinearClamp, uv + size * float2(0, 0));

    float4 color = 0;
    color += c0 * 0.11111111;
    color += c1 * 0.11111111;
    color += c2 * 0.11111111;
    color += c3 * 0.11111111;
    color += c4 * 0.11111111;
    color += c5 * 0.11111111;
    color += c6 * 0.11111111;
    color += c7 * 0.11111111;
    color += c8 * 0.11111111;

    return color;
}