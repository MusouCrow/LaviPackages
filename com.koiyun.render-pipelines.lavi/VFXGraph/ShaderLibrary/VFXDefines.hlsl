#include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"

// this is only necessary for the old VFXTarget pathway
// it defines the macro used to access hybrid instanced properties
// (new HDRP/URP Target pathway overrides the type so this is never used)
#define UNITY_ACCESS_HYBRID_INSTANCED_PROP(name, type) name
