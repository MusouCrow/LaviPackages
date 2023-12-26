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

TransparentOutput Frag(PackedVaryings packedInput)
{
    TransparentOutput output;
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);

    SurfaceDescription surfaceDescription = BuildSurfaceDescription(unpacked);

    float4 color = float4(surfaceDescription.BaseColor, surfaceDescription.Alpha);

#ifdef _ADDITIVE
    color.rgb *= color.a;
#endif

    output.color = color;
    output.glow = float4(color.rgb * surfaceDescription.Glow * 10, color.a);

    return output;
}
