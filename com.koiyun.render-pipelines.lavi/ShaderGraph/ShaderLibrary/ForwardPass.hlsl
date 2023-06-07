#pragma once

#include "./Varyings.hlsl"
#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Shadow.hlsl"

PackedVaryings Vert(Attributes input)
{
    Varyings output;
    output = BuildVaryings(input);
    PackedVaryings packedOutput = PackVaryings(output);

    return packedOutput;
}

half4 Frag(PackedVaryings packedInput) : SV_TARGET
{
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);

    SurfaceDescription surfaceDescription = BuildSurfaceDescription(unpacked);

    half3 color = surfaceDescription.BaseColor;

#ifdef _MAIN_LIGHT_SHADOWS
    float shadowAttenuation = ShadowAttenuation(unpacked.positionWS);
    color *= shadowAttenuation;
#endif

    return half4(color, 1);
}
