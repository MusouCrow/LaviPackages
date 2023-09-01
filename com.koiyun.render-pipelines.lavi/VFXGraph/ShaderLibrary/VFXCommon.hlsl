#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"

#ifdef VFX_VARYING_PS_INPUTS
    void VFXTransformPSInputs(inout VFX_VARYING_PS_INPUTS input) {}

    float4 VFXApplyPreExposure(float4 color, float exposureWeight)
    {
        return color;
    }

    float4 VFXApplyPreExposure(float4 color, VFX_VARYING_PS_INPUTS input)
    {
        return color;
    }
#endif

float4 VFXTransformFinalColor(float4 color)
{
    return color;
}

float2 VFXGetNormalizedScreenSpaceUV(float4 positionCS)
{
    return GetNormalizedScreenSpaceUV(positionCS);
}

// TODO
void VFXEncodeMotionVector(float2 velocity, out float4 outBuffer)
{
    outBuffer = (float4)0.0f;
}

float4 VFXTransformPositionWorldToClip(float3 positionWS)
{
    return TransformWorldToHClip(positionWS);
}

float4 VFXTransformPositionWorldToNonJitteredClip(float3 positionWS)
{
    return VFXTransformPositionWorldToClip(positionWS);
}

float4 VFXTransformPositionWorldToPreviousClip(float3 positionWS)
{
    return VFXTransformPositionWorldToClip(positionWS);
}

float4 VFXTransformPositionObjectToClip(float3 positionOS)
{
    float3 positionWS = TransformObjectToWorld(positionOS);

    return VFXTransformPositionWorldToClip(positionWS);
}

float4 VFXTransformPositionObjectToNonJitteredClip(float3 positionOS)
{
    return VFXTransformPositionObjectToClip(positionOS);
}

float4 VFXTransformPositionObjectToPreviousClip(float3 positionOS)
{
    return VFXTransformPositionObjectToClip(positionOS);
}

float3 VFXTransformPositionWorldToView(float3 positionWS)
{
    return TransformWorldToView(positionWS);
}

float3 VFXTransformPositionWorldToCameraRelative(float3 positionWS)
{
    return positionWS;
}

float4x4 ApplyCameraTranslationToMatrix(float4x4 modelMatrix)
{
    return modelMatrix;
}

float4x4 ApplyCameraTranslationToInverseMatrix(float4x4 inverseModelMatrix)
{
    return inverseModelMatrix;
}

float4x4 GetRawUnityObjectToWorld()
{
    return unity_ObjectToWorld;
}

float4x4 GetRawUnityWorldToObject()
{
    return unity_WorldToObject;
}

float4x4 VFXGetObjectToWorldMatrix()
{
    // NOTE: If using the new generation path, explicitly call the object matrix (since the particle matrix is now baked into UNITY_MATRIX_M)
#ifdef HAVE_VFX_MODIFICATION
    return GetRawUnityObjectToWorld();
#else
    return GetObjectToWorldMatrix();
#endif
}

float4x4 VFXGetWorldToObjectMatrix()
{
    // NOTE: If using the new generation path, explicitly call the object matrix (since the particle matrix is now baked into UNITY_MATRIX_I_M)
#ifdef HAVE_VFX_MODIFICATION
    return GetRawUnityWorldToObject();
#else
    return GetWorldToObjectMatrix();
#endif
}

float3x3 VFXGetWorldToViewRotMatrix()
{
    return (float3x3)GetWorldToViewMatrix();
}

// TODO
float3 VFXGetViewWorldPosition()
{
    return 0;
}

float4x4 VFXGetViewToWorldMatrix()
{
    return UNITY_MATRIX_I_V;
}

#ifdef USING_STEREO_MATRICES
    float3 GetWorldStereoOffset()
    {
        return float3(0.0f, 0.0f, 0.0f);
    }
#endif

// TODO
float VFXSampleDepth(float4 positionSS)
{
    return 0;
}

// TODO
void VFXApplyShadowBias(inout float4 positionCS, inout float3 positionWS, float3 normalWS)
{
    
}

// TODO
void VFXApplyShadowBias(inout float4 positionCS, inout float3 positionWS)
{
    
}

float4 VFXApplyFog(float4 color, float4 positionCS, float3 positionWS)
{
    return color;
}

float4 VFXApplyAO(float4 color, float4 positionCS)
{
    return color;
}

// TODO
float3 VFXGetCameraWorldDirection()
{
    return 0;
}

#define VFXComputePixelOutputToNormalBuffer(i,normalWS,uvData,outNormalBuffer) \
{ \
    outNormalBuffer = float4(normalWS, 0.0); \
}
