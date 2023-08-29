#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"

TEXTURE2D(_MainTex);
SAMPLER(sampler_PointClamp);
SAMPLER(sampler_LinearClamp);

struct Attributes
{
    float4 positionOS : POSITION;
    float2 texcoord : TEXCOORD0;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
};

Varyings Vert(Attributes input)
{
    Varyings output;
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
    output.uv = input.texcoord;

    return output;
}

half4 Frag(Varyings input) : SV_Target
{
#ifdef _POINT_FILTER
    half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_PointClamp, input.uv);
#else
    half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, input.uv);
#endif

    return color;
}
