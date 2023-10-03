#pragma once

float3 GetCurrentViewPosition()
{
    return _WorldSpaceCameraPos;
}

float3 GetWorldSpaceNormalizeViewDir(float3 positionWS)
{
    float3 V = GetCurrentViewPosition() - positionWS;

    return normalize(V);
}
