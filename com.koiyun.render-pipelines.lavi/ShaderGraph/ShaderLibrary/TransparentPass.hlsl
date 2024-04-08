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

    float4 color = float4(surfaceDescription.BaseColor, surfaceDescription.Alpha);

#ifdef _ADDITIVE
    color.rgb *= color.a;
#endif

    output.color = color;
    output.param = float4(surfaceDescription.Glow, 0, 0, color.a);

    return output;
}
