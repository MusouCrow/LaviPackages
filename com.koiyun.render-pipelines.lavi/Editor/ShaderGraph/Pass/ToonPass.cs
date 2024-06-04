using System.Collections.Generic;
using UnityEditor.ShaderGraph;

namespace Koiyun.Render.ShaderGraph.Editor {
    static class ToonPass {
        public static SubShaderDescriptor SubShader(ToonSubTarget subTarget, string renderType, string renderQueue) {
            var passes = new PassCollection() {
                Opaque(subTarget, false)
            };

            if (subTarget.target.occlusion > StencilType.None) {
                passes.Add(Opaque(subTarget, true));
            }
            
            if (subTarget.shadowCasterPass) {
                passes.Add(ShadowCaster(subTarget));
            }
            
            return new SubShaderDescriptor() {
                renderType = renderType,
                renderQueue = renderQueue,
                generatesPreview = true,
                passes = passes
            };
        }

        public static PassDescriptor Opaque(ToonSubTarget subTarget, bool occlusion) {
            var keywords = new KeywordCollection();
            var defines = new DefineCollection();
            var requiredFields = new FieldCollection();
            var validPixelBlocks = new List<BlockFieldDescriptor>() {
                BlockFields.SurfaceDescription.BaseColor,
                ShaderPropertyUtil.SurfaceDescription.Glow,
                ShaderPropertyUtil.SurfaceDescription.Layer
            };

            if (occlusion) {
                validPixelBlocks.Add(BlockFields.SurfaceDescription.AlphaClipThreshold);
                defines.Add(ShaderPropertyUtil.NeedAlphaClipKeyword, 1);
                defines.Add(ShaderPropertyUtil.OcclusionKeywordDefined, 1);
                requiredFields.Add(StructFields.Varyings.screenPosition);
            }
            else if (subTarget.alphaClipMode > 0) {
                validPixelBlocks.Add(BlockFields.SurfaceDescription.AlphaClipThreshold);

                if (subTarget.alphaClipMode == AlphaClipMode.Switch) {
                    keywords.Add(ShaderPropertyUtil.AlphaClipKeyword);
                }
                else if (subTarget.alphaClipMode == AlphaClipMode.Enabled) {
                    defines.Add(ShaderPropertyUtil.NeedAlphaClipKeyword, 1);
                }
            }

            if (subTarget.shadowCasterPass && !occlusion) {
                keywords.Add(ShaderPropertyUtil.MainLightShadowsKeyword);
            }

            var stencilType = occlusion ? subTarget.target.occlusion : StencilType.None;
            var stencilTest = occlusion;
            var renderState = ShaderPropertyUtil.GetRenderState(subTarget.target, false, stencilType, stencilTest);

            return new PassDescriptor() {
                // Definition
                displayName = occlusion ? "OcclusionOpaque" : "Opaque",
                referenceName = occlusion ? "SHADERPASS_OCCLUSION_OPAQUE" : "SHADERPASS_OPAQUE",
                lightMode = occlusion ? "OcclusionOpaque" : "Opaque",
                useInPreview = !occlusion,

                // Template
                passTemplatePath = ShaderGraphConst.SHADER_PASS_PATH,
                sharedTemplateDirectories = new string[] {
                    ShaderGraphConst.INNER_TEMPLATE_PATH,
                    ShaderGraphConst.TEMPLATE_PATH
                },

                // Port Mask
                validVertexBlocks = new BlockFieldDescriptor[] { BlockFields.VertexDescription.Position, BlockFields.VertexDescription.Normal, BlockFields.VertexDescription.Tangent },
                validPixelBlocks = validPixelBlocks.ToArray(),

                // Fields
                structs = new StructCollection() {
                    Structs.Attributes,
                    LaviStructs.Varyings,
                    Structs.SurfaceDescriptionInputs,
                    Structs.VertexDescriptionInputs
                },
                requiredFields = requiredFields,
                fieldDependencies = new DependencyCollection() {
                    FieldDependencies.Default
                },

                // Conditional State
                renderStates = renderState,
                pragmas = new PragmaCollection() {
                    Pragma.Vertex("Vert"), 
                    Pragma.Fragment("Frag"), 
                    Pragma.MultiCompileInstancing
                },
                defines = defines,
                keywords = keywords,
                includes = new IncludeCollection() {
                    {ShaderGraphConst.SHADERLIB_CORE, IncludeLocation.Pregraph},
                    {ShaderGraphConst.SHADERLIB_SG_FUNCTIONS, IncludeLocation.Pregraph},
                    {ShaderGraphConst.SHADERLIB_FUNCTIONS, IncludeLocation.Pregraph},
                    {ShaderGraphConst.SHADERLIB_OPAQUE_PASS, IncludeLocation.Postgraph},
                },
            };
        }
        
        public static PassDescriptor ShadowCaster(ToonSubTarget subTarget) {
            var defines = new DefineCollection();
            var keywords = new KeywordCollection();
            var validPixelBlocks = new List<BlockFieldDescriptor>();

            if (subTarget.target.surfaceType == SurfaceType.Opaque) {
                validPixelBlocks.Add(BlockFields.SurfaceDescription.AlphaClipThreshold);

                if (subTarget.alphaClipMode == AlphaClipMode.Switch) {
                    keywords.Add(ShaderPropertyUtil.AlphaClipKeyword);
                }
                else if (subTarget.alphaClipMode == AlphaClipMode.Enabled) {
                    defines.Add(ShaderPropertyUtil.NeedAlphaClipKeyword, 1);
                }
            }

            return new PassDescriptor() {
                // Definition
                displayName = "ShadowCaster",
                referenceName = "SHADERPASS_SHADOWCASTER",
                lightMode = "ShadowCaster",

                // Template
                passTemplatePath = ShaderGraphConst.SHADER_PASS_PATH,
                sharedTemplateDirectories = new string[] {
                    ShaderGraphConst.INNER_TEMPLATE_PATH,
                    ShaderGraphConst.TEMPLATE_PATH
                },

                // Port Mask
                validVertexBlocks = new BlockFieldDescriptor[] { BlockFields.VertexDescription.Position, BlockFields.VertexDescription.Normal, BlockFields.VertexDescription.Tangent },
                validPixelBlocks = validPixelBlocks.ToArray(),

                // Fields
                structs = new StructCollection() {
                    Structs.Attributes,
                    LaviStructs.Varyings,
                    Structs.SurfaceDescriptionInputs,
                    Structs.VertexDescriptionInputs
                },
                requiredFields = new FieldCollection() {
                    StructFields.Attributes.normalOS
                },
                fieldDependencies = new DependencyCollection() {
                    FieldDependencies.Default
                },

                // Conditional State
                renderStates = ShaderPropertyUtil.GetRenderState(subTarget.target, true),
                
                pragmas = new PragmaCollection() {
                    Pragma.Vertex("Vert"), 
                    Pragma.Fragment("Frag"), 
                    Pragma.MultiCompileInstancing
                },
                defines = defines,
                keywords = keywords,
                includes = new IncludeCollection() {
                    {ShaderGraphConst.SHADERLIB_CORE, IncludeLocation.Pregraph},
                    {ShaderGraphConst.SHADERLIB_FUNCTIONS, IncludeLocation.Pregraph},
                    {ShaderGraphConst.SHADERLIB_SHADOWCASTER_PASS, IncludeLocation.Postgraph},
                },
            };
        }
    }
}