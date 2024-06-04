#pragma once

#include "./Varyings.hlsl"
#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"

PackedVaryings Vert(Attributes input)
{
    Varyings output;
    output = BuildVaryings(input);
    PackedVaryings packedOutput = PackVaryings(output);

    return packedOutput;
}

Output Frag(PackedVaryings packedInput)
{
    Output output;
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);

    SurfaceDescription surfaceDescription = BuildSurfaceDescription(unpacked);

#ifdef _OCCLUSION
    OcclusionOpaque(surfaceDescription, unpacked.screenPosition);
#endif

    AlphaClip(surfaceDescription);

#ifdef _OCCLUSION
    output.color = float4(surfaceDescription.BaseColor * 0.1, 1);
    output.param = float4(0, 0, 0, 1);
#else
    output.color = float4(surfaceDescription.BaseColor, 1);
    output.param = float4(surfaceDescription.Glow, surfaceDescription.Layer, 0, 1);
#endif

    return output;
}
