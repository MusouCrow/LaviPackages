#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

TEXTURE2D(_ParamTexture);

float4 _ParamTexture_TexelSize;
float2 _OutlineParams; // OutlineBrightness, OutlineThickness
float _RenderScale;

struct Attributes
{
    uint vertexID : SV_VertexID;
    float4 positionOS : POSITION;
    float2 texcoord : TEXCOORD0;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
};

float SobelLayer(float2 uv)
{
    const half2 DX[9] = {
        {0, 0},
        {-1, 0}, {-2, 0},
        {1, 0}, {2, 0},
        {0, -1}, {0, -2},
        {0, 1}, {0, 2},
    };

    const half SO[9] = {
        8,
        -1, -1,
        -1, -1,
        -1, -1,
        -1, -1
    };

    float scale = (_ParamTexture_TexelSize.w / (1080 * 2)) * _OutlineParams.y;
    float edge = 0;

    for (int i = 0; i < 9; i++) {
        
        float2 _uv = uv + DX[i] * _ParamTexture_TexelSize.xy * scale;
        float layer = SAMPLE_TEXTURE2D_LOD(_ParamTexture, sampler_LinearClamp, _uv, 0).g;
        edge += layer * SO[i];
    }

    edge = step(0.01, edge);

    return edge;
}

Varyings Vert(Attributes input)
{
    Varyings output;
    output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
    output.uv = GetFullScreenTriangleTexCoord(input.vertexID);

    return output;
}

float4 Frag(Varyings input) : SV_TARGET
{
    float4 param = SAMPLE_TEXTURE2D_LOD(_ParamTexture, sampler_PointClamp, input.uv, 0);
    float layer = param.g;
    
    float edge = SobelLayer(input.uv);
    float noOutline = 1 - step(1, layer);

    // float3 hsv = RgbToHsv(output.color.rgb);
    // float rate = lerp(lerp(lerp(_OutlineParams.x * 2, _OutlineParams.x * 0.5, hsv.b), 1, noOutline), 1, (1 - edge));
    
    param.g = edge * noOutline;

    return param;
}
