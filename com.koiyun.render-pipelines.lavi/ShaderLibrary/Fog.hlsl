#pragma once

float2 _FogRange;
float4 _FogColor;

float3 HighFog(float3 color, float3 positionWS)
{
    float bottom = _FogRange.x;
    float top = _FogRange.y;
    float range = top - bottom;
    float y = positionWS.y;
    float high = y - bottom;
    float rate = saturate(high / range);
    color = lerp(_FogColor.rgb, color, rate);

    return color;
}
