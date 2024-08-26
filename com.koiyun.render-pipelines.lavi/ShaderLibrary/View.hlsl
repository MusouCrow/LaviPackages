#pragma once

float3 _CameraPosOS;

float3 GetCurrentViewPosition()
{
    return _WorldSpaceCameraPos;
}

float3 GetWorldSpaceNormalizeViewDir(float3 positionWS)
{
    float3 V = GetCurrentViewPosition() - positionWS;

    return normalize(V);
}

float3 GetOriginSpaceNormalizeViewDir(float3 positionWS)
{
    float3 V = _CameraPosOS - positionWS;

    return normalize(V);
}

// Returns 'true' if the current view performs a perspective projection.
bool IsPerspectiveProjection()
{
    return (unity_OrthoParams.w == 0);
}
