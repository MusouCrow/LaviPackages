// #include "Packages/com.unity.visualeffectgraph/Shaders/VFXMatricesOverride.hlsl"
#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"

#define CameraBuffer Texture2D
#define VFXSamplerCameraBuffer VFXSampler2D

// this is only necessary for the old VFXTarget pathway
// it defines the macro used to access hybrid instanced properties
// (new HDRP/URP Target pathway overrides the type so this is never used)
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(name, type) name

//Unlit can use the DepthNormal pass which creates a discrepancy while computing depth
#define FORCE_NORMAL_OUTPUT_UNLIT_VERTEX_SHADER 1
