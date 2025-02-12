#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Screen.hlsl"

void AlphaClip(SurfaceDescription surfaceDescription)
{
#if defined(SURFACE_NEED_ALPHA_CLIP) || defined(_ALPHA_CLIP)
    clip(saturate(surfaceDescription.AlphaClipThreshold) - 0.001);
#endif
}

void OcclusionOpaque(inout SurfaceDescription surfaceDescription, float4 positionCS)
{
#if defined(SURFACE_NEED_ALPHA_CLIP) || defined(_ALPHA_CLIP)
    float4 positionNDC = ComputeNDCPosition(positionCS);
    surfaceDescription.AlphaClipThreshold = Dither(0.25, positionNDC) * step(0.01, surfaceDescription.AlphaClipThreshold);
#endif
}
