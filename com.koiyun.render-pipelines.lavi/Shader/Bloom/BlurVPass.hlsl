#pragma once

#include "./Include.hlsl"

float4 Frag(Varyings input) : SV_Target
{
    float texelSize = _MainTex_TexelSize.y;
    float2 uv = input.uv;

    float4 c0 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - float2(0.0, texelSize * 3.23076923));
    float4 c1 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - float2(0.0, texelSize * 1.38461538));
    float4 c2 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv);
    float4 c3 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + float2(0.0, texelSize * 1.38461538));
    float4 c4 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + float2(0.0, texelSize * 3.23076923));

    float4 color = 0;
    color += HandleColor(c0, 0.07027027);
    color += HandleColor(c1, 0.31621622);
    color += HandleColor(c2, 0.22702703);
    color += HandleColor(c3, 0.31621622);
    color += HandleColor(c4, 0.07027027);
    
    return color;
}

