#pragma once

void AlphaClip(SurfaceDescription surfaceDescription)
{
#if defined(SURFACE_NEED_ALPHA_CLIP) || defined(_ALPHA_CLIP)
    clip(surfaceDescription.AlphaClipThreshold - 0.001);
#endif
}
