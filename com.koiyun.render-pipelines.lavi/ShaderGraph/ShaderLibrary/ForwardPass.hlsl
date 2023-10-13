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

    half4 color = half4(surfaceDescription.BaseColor, 1);

#ifdef SURFACE_NEED_ALPHA
    color.a = surfaceDescription.Alpha;
#endif

#ifdef _ADDITIVE
    color.rgb *= color.a;
#endif

    AlphaClip(surfaceDescription);

    output.color = color;
    output.glow = float4(surfaceDescription.Glow, 0, 0, color.a);

    return output;
}
