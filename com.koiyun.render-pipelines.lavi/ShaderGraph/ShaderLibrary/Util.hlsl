#pragma once

#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Screen.hlsl"

void AlphaClip(SurfaceDescription surfaceDescription)
{
#if defined(SURFACE_NEED_ALPHA_CLIP) || defined(_ALPHA_CLIP)
    clip(surfaceDescription.AlphaClipThreshold);
#endif
}

void OcclusionOpaque(inout SurfaceDescription surfaceDescription, float4 screenPos)
{
#if defined(SURFACE_NEED_ALPHA_CLIP) || defined(_ALPHA_CLIP)
    surfaceDescription.AlphaClipThreshold = Dither(0.1, screenPos);
#endif
}
