#pragma once

#include "./Include.hlsl"

float4 Frag(Varyings input) : SV_Target
{
    float texelSize = _MainTex_TexelSize.x * 2.0;
    float2 uv = input.uv;

    float4 c0 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - float2(texelSize * 4.0, 0.0));
    float4 c1 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - float2(texelSize * 3.0, 0.0));
    float4 c2 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - float2(texelSize * 2.0, 0.0));
    float4 c3 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - float2(texelSize * 1.0, 0.0));
    float4 c4 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv);
    float4 c5 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + float2(texelSize * 1.0, 0.0));
    float4 c6 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + float2(texelSize * 2.0, 0.0));
    float4 c7 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + float2(texelSize * 3.0, 0.0));
    float4 c8 = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + float2(texelSize * 4.0, 0.0));

    float4 color = 0;
    color += HandleColor(c0, 0.01621622);
    color += HandleColor(c1, 0.05405405);
    color += HandleColor(c2, 0.12162162);
    color += HandleColor(c3, 0.19459459);
    color += HandleColor(c4, 0.22702703);
    color += HandleColor(c5, 0.19459459);
    color += HandleColor(c6, 0.12162162);
    color += HandleColor(c7, 0.05405405);
    color += HandleColor(c8, 0.01621622);

    return color;
}