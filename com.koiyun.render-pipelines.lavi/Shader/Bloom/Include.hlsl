#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"

SAMPLER(sampler_LinearClamp);

TEXTURE2D(_MainTex);
TEXTURE2D(_CameraGlowTexture);
TEXTURE2D(_BloomBlurTexture);

CBUFFER_START(UnityPerMaterial)
float4 _MainTex_TexelSize;
CBUFFER_END

struct Attributes
{
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
};

Varyings Vert(Attributes input)
{
    Varyings output;

    float3 positionWS = TransformObjectToWorld(input.positionOS);
    output.positionCS = TransformWorldToHClip(positionWS);
    output.uv = input.uv;

    return output;
}

float4 HandleColor(float4 color, float rate)
{
    return color * rate;
}
