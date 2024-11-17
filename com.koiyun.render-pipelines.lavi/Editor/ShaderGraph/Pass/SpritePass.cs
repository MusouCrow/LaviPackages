using System.Collections.Generic;
using UnityEditor.ShaderGraph;

namespace Koiyun.Render.ShaderGraph.Editor {
    static class SpritePass {
        public static SubShaderDescriptor SubShader(SpriteSubTarget subTarget, string renderType, string renderQueue) {
            var passes = new PassCollection() {
                Transparent(subTarget, false)
            };

            if (subTarget.target.occlusion > StencilType.None) {
                passes.Add(Transparent(subTarget, true));
            }
            
            return new SubShaderDescriptor() {
                renderType = renderType,
                renderQueue = renderQueue,
                generatesPreview = true,
                passes = passes
            };
        }

        public static PassDescriptor Transparent(SpriteSubTarget subTarget, bool occlusion) {
            var keywords = new KeywordCollection();
            var defines = new DefineCollection();
            var validPixelBlocks = new List<BlockFieldDescriptor>() {
                BlockFields.SurfaceDescription.BaseColor,
                BlockFields.SurfaceDescription.Alpha,
                ShaderPropertyUtil.SurfaceDescription.Glow
            };

            if (subTarget.target.blendMode == BlendMode.Additive && !subTarget.target.overrideBlendMode) {
                defines.Add(ShaderPropertyUtil.AdditiveKeywordDefined, 1);
            }
            else {
                keywords.Add(ShaderPropertyUtil.AdditiveKeyword);
            }

            if (occlusion) {
                defines.Add(ShaderPropertyUtil.OcclusionKeywordDefined, 1);
            }
            
            var lightMode = subTarget.target.ui ? "UI" : "Transparent";

            return new PassDescriptor() {
                // Definition
                displayName = occlusion ? "OcclusionTransparent" : "Transparent",
                referenceName = occlusion ? "SHADERPASS_OCCLUSION_TRANSPARENT" : "SHADERPASS_TRANSPARENT",
                lightMode = occlusion ? "Occlusion" + lightMode : lightMode,
                useInPreview = true,

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
                requiredFields = new FieldCollection(),
                fieldDependencies = new DependencyCollection() {
                    FieldDependencies.Default
                },

                // Conditional State
                renderStates = ShaderPropertyUtil.GetRenderState(subTarget.target),
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
                    {ShaderGraphConst.SHADERLIB_TRANSPARENT_PASS, IncludeLocation.Postgraph},
                },
            };
        }
    }
}