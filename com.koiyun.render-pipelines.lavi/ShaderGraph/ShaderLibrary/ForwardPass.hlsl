#pragma once

#include "./Varyings.hlsl"

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

    half4 color = half4(surfaceDescription.BaseColor, 1);

#ifdef SURFACE_NEED_ALPHA
    color.a = surfaceDescription.Alpha;
#endif

    AlphaClip(surfaceDescription);

    return color;
}
