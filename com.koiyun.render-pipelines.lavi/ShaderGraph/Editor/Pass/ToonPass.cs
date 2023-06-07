using UnityEditor.ShaderGraph;

namespace Koiyun.Render.ShaderGraph.Editor {
    static class ToonPasses {
        public static SubShaderDescriptor SubShader(LaviTarget target, string renderType, string renderQueue) {
            var passes = new PassCollection() {
                Forward(target)
            };

            if (target.shadowCasterPass) {
                passes.Add(ShadowCaster(target));
            }
            
            return new SubShaderDescriptor() {
                renderType = renderType,
                renderQueue = renderQueue,
                generatesPreview = true,
                passes = passes
            };
        }

        public static PassDescriptor Forward(LaviTarget target) {
            var keywords = new KeywordCollection();
            var requiredFields = new FieldCollection();

            if (target.shadowCasterPass) {
                keywords.Add(ShaderPropertyUtil.MainLightShadowsKeyword);
                keywords.Add(ShaderPropertyUtil.ShadowSoftKeyword);
                requiredFields.Add(StructFields.Varyings.positionWS);
            }

            return new PassDescriptor() {
                // Definition
                displayName = "Forward",
                referenceName = "SHADERPASS_FORWARD",
                lightMode = "Forward",
                useInPreview = true,

                // Template
                passTemplatePath = ShaderGraphConst.SHADER_PASS_PATH,
                sharedTemplateDirectories = new string[] {
                    ShaderGraphConst.INNER_TEMPLATE_PATH,
                    ShaderGraphConst.TEMPLATE_PATH
                },

                // Port Mask
                validVertexBlocks = new BlockFieldDescriptor[] {},
                validPixelBlocks = new BlockFieldDescriptor[] {BlockFields.SurfaceDescription.BaseColor},

                // Fields
                structs = new StructCollection() {
                    Structs.Attributes,
                    LaviStructs.Varyings,
                    Structs.SurfaceDescriptionInputs,
                    Structs.VertexDescriptionInputs
                },
                requiredFields = requiredFields,
                fieldDependencies = new DependencyCollection(),

                // Conditional State
                renderStates = GetRenderState(target, true, true, true, true),
                pragmas = new PragmaCollection() {
                    Pragma.Vertex("Vert"), 
                    Pragma.Fragment("Frag"), 
                    Pragma.MultiCompileInstancing
                },
                defines = new DefineCollection(),
                keywords = keywords,
                includes = new IncludeCollection() {
                    {ShaderGraphConst.SHADERLIB_CORE, IncludeLocation.Pregraph},
                    {ShaderGraphConst.SHADERLIB_FUNCTIONS, IncludeLocation.Pregraph},
                    {ShaderGraphConst.SHADERLIB_FORWARD_PASS, IncludeLocation.Postgraph},
                },
            };
        }

        public static PassDescriptor ShadowCaster(LaviTarget target) {
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
                validVertexBlocks = new BlockFieldDescriptor[] {},
                validPixelBlocks = new BlockFieldDescriptor[] {},

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
                fieldDependencies = new DependencyCollection(),

                // Conditional State
                renderStates = GetRenderState(target, false, true, false, false),
                pragmas = new PragmaCollection() {
                    Pragma.Vertex("Vert"), 
                    Pragma.Fragment("Frag"), 
                    Pragma.MultiCompileInstancing
                },
                defines = new DefineCollection(),
                keywords = new KeywordCollection() {},
                includes = new IncludeCollection() {
                    {ShaderGraphConst.SHADERLIB_CORE, IncludeLocation.Pregraph},
                    {ShaderGraphConst.SHADERLIB_FUNCTIONS, IncludeLocation.Pregraph},
                    {ShaderGraphConst.SHADERLIB_SHADOWCASTER_PASS, IncludeLocation.Postgraph},
                },
            };
        }

        private static RenderStateCollection GetRenderState(LaviTarget target, bool blendMode, bool cullMode, bool zWrite, bool zTest) {
            var result = new RenderStateCollection();
            
            if (blendMode) {
                if (target.overrideBlendMode) {
                    var srcBlend = "[" + ShaderGraphConst.SRC_BLEND_PROPERTY + "]";
                    var dstBlend = "[" + ShaderGraphConst.DST_BLEND_PROPERTY + "]";
                    result.Add(RenderState.Blend(srcBlend, dstBlend));
                }
                else {
                    target.GetBlend(out var srcBlend, out var dstBlend);
                    result.Add(RenderState.Blend(srcBlend, dstBlend));
                }
            }

            if (cullMode) {
                if (target.overrideCullMode) {
                    var value = "[" + ShaderGraphConst.CULL_PROPERTY + "]";
                    result.Add(RenderState.Cull(value));
                }
                else {
                    result.Add(RenderState.Cull(target.cullMode.ToString()));
                }
            }

            if (zWrite) {
                if (target.overrideZWrite) {
                    var value = "[" + ShaderGraphConst.ZWRITE_PROPERTY + "]";
                    result.Add(RenderState.ZWrite(value));
                }
                else {
                    result.Add(RenderState.ZWrite(target.zWrite ? ZWrite.On : ZWrite.Off));
                }
            }

            if (zTest) {
                if (target.overrideZTest) {
                    var value = "[" + ShaderGraphConst.ZTEST_PROPERTY + "]";
                    result.Add(RenderState.ZTest(value));
                }
                else {
                    result.Add(RenderState.ZTest(target.zTest));
                }
            }
            
            return result;
        }
    }
}