#pragma once

#include "./Varyings.hlsl"
#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Shadow.hlsl"

Varyings BuildVaryingsShadowCaster(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);

#ifdef FEATURES_GRAPH_VERTEX
    VertexDescription vertexDescription = BuildVertexDescription(input);
    
    #ifdef ATTRIBUTES_NEED_POSITION
        input.positionOS = vertexDescription.Position;
    #endif

    #ifdef VARYINGS_NEED_NORMAL_WS
        input.normalOS = vertexDescription.Normal;
    #endif

    #ifdef VARYINGS_NEED_TANGENT_WS
        input.tangentOS = vertexDescription.Tangent;
    #endif
#endif

float3 normalWS = TransformObjectToWorldNormal(input.positionOS);
float3 positionWS = TransformObjectToWorld(input.positionOS);
positionWS = ApplyShadowBias(positionWS, normalWS);

#ifdef VARYINGS_NEED_POSITION_WS
    output.positionWS = positionWS;
#endif

#ifdef VARYINGS_NEED_NORMAL_WS
    output.normalWS = normalWS;
#endif

#ifdef VARYINGS_NEED_TANGENT_WS
    output.tangentWS = float4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
#endif

float4 positionCS = TransformWorldToHClip(positionWS);

#if UNITY_REVERSED_Z
    positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#else
    positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#endif

output.positionCS = positionCS;

#ifdef VARYINGS_NEED_TEXCOORD0
    output.texCoord0 = input.uv0;
#endif

#ifdef VARYINGS_NEED_TEXCOORD1
    output.texCoord1 = input.uv1;
#endif

#ifdef VARYINGS_NEED_TEXCOORD2
    output.texCoord2 = input.uv2;
#endif

#ifdef VARYINGS_NEED_TEXCOORD3
    output.texCoord3 = input.uv3;
#endif

#ifdef VARYINGS_NEED_COLOR
    output.color = input.color;
#endif

#ifdef VARYINGS_NEED_VIEWDIRECTION_WS
    output.viewDirectionWS = GetWorldSpaceViewDir(positionWS);
#endif

#ifdef VARYINGS_NEED_SCREENPOSITION
    output.screenPosition = ComputeScreenPos(output.positionCS);
#endif

    return output;
}

PackedVaryings Vert(Attributes input)
{
    Varyings output;
    output = BuildVaryingsShadowCaster(input);
    PackedVaryings packedOutput = PackVaryings(output);

    return packedOutput;
}

half4 Frag(PackedVaryings packedInput) : SV_TARGET
{
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);

    SurfaceDescription surfaceDescription = BuildSurfaceDescription(unpacked);

    return 0;
}
