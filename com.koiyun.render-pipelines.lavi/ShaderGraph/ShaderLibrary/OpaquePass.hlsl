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

OpaqueOutput Frag(PackedVaryings packedInput)
{
    OpaqueOutput output;
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);

    SurfaceDescription surfaceDescription = BuildSurfaceDescription(unpacked);
    
    AlphaClip(surfaceDescription);

    output.color = float4(surfaceDescription.BaseColor, surfaceDescription.Layer);
    output.glow = float4(surfaceDescription.BaseColor * surfaceDescription.Glow * 10, 1);

    return output;
}
