using System.Collections.Generic;
using UnityEditor.ShaderGraph;

namespace Koiyun.Render.ShaderGraph.Editor {
    static class ToonPasses {
        public static SubShaderDescriptor SubShader(ToonSubTarget subTarget, string renderType, string renderQueue) {
            var passes = new PassCollection() {
                Forward(subTarget)
            };

            if (subTarget.outlinePass) {
                passes.Add(Outline(subTarget));
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

        public static PassDescriptor Forward(ToonSubTarget subTarget) {
            var keywords = new KeywordCollection();
            var requiredFields = new FieldCollection();
            var defines = new DefineCollection();
            var validPixelBlocks = new List<BlockFieldDescriptor>() {
                BlockFields.SurfaceDescription.BaseColor,
                ShaderPropertyUtil.SurfaceDescription.Glow
            };

            var surfaceType = subTarget.target.surfaceType;

            if (surfaceType == SurfaceType.Transparent) {
                validPixelBlocks.Add(BlockFields.SurfaceDescription.Alpha);
                defines.Add(ShaderPropertyUtil.NeedAlphaKeyword, 1);

                if (subTarget.target.blendMode == BlendMode.Additive && !subTarget.target.overrideBlendMode) {
                    defines.Add(ShaderPropertyUtil.AdditiveKeywordDefined, 1);
                }
                else {
                    keywords.Add(ShaderPropertyUtil.AdditiveKeyword);
                }                
            }
            else if (surfaceType == SurfaceType.Opaque) {
                validPixelBlocks.Add(BlockFields.SurfaceDescription.AlphaClipThreshold);

                if (subTarget.alphaClipMode == AlphaClipMode.Swtich) {
                    keywords.Add(ShaderPropertyUtil.AlphaClipKeyword);
                }
                else if (subTarget.alphaClipMode == AlphaClipMode.Enabled) {
                    defines.Add(ShaderPropertyUtil.NeedAlphaClipKeyword, 1);
                }
            }
            
            if (subTarget.shadowCasterPass) {
                keywords.Add(ShaderPropertyUtil.MainLightShadowsKeyword);
                keywords.Add(ShaderPropertyUtil.ShadowSoftKeyword);
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
                renderStates = GetRenderState(subTarget.target, true, true, true, true, true, false),
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
                    {ShaderGraphConst.SHADERLIB_FORWARD_PASS, IncludeLocation.Postgraph},
                },
            };
        }

        public static PassDescriptor Outline(ToonSubTarget subTarget) {
            var defines = new DefineCollection();
            var keywords = new KeywordCollection();
            var validPixelBlocks = new List<BlockFieldDescriptor>() {
                ShaderPropertyUtil.SurfaceDescription.OutlineColor
            };

            if (subTarget.target.surfaceType == SurfaceType.Opaque) {
                validPixelBlocks.Add(BlockFields.SurfaceDescription.AlphaClipThreshold);

                if (subTarget.alphaClipMode == AlphaClipMode.Swtich) {
                    keywords.Add(ShaderPropertyUtil.AlphaClipKeyword);
                }
                else if (subTarget.alphaClipMode == AlphaClipMode.Enabled) {
                    defines.Add(ShaderPropertyUtil.NeedAlphaClipKeyword, 1);
                }
            }

            if (subTarget.shadowCasterPass) {
                keywords.Add(ShaderPropertyUtil.MainLightShadowsKeyword);
                keywords.Add(ShaderPropertyUtil.ShadowSoftKeyword);
            }

            return new PassDescriptor() {
                // Definition
                displayName = "Outline",
                referenceName = "SHADERPASS_OUTLINE",
                lightMode = "Outline",

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
                renderStates = new RenderStateCollection() {
                    RenderState.Cull(Cull.Front)
                },
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
                    {ShaderGraphConst.SHADERLIB_OUTLINE_PASS, IncludeLocation.Postgraph},
                },
            };
        }

        public static PassDescriptor ShadowCaster(ToonSubTarget subTarget) {
            var defines = new DefineCollection();
            var keywords = new KeywordCollection();
            var validPixelBlocks = new List<BlockFieldDescriptor>();

            if (subTarget.target.surfaceType == SurfaceType.Opaque) {
                validPixelBlocks.Add(BlockFields.SurfaceDescription.AlphaClipThreshold);

                if (subTarget.alphaClipMode == AlphaClipMode.Swtich) {
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
                renderStates = GetRenderState(subTarget.target, false, true, false, false, false, true),
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

        private static RenderStateCollection GetRenderState(LaviTarget target, bool blendMode, bool cullMode, bool zWrite, bool zTest, bool stencil, bool colorMask) {
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

            if (stencil && target.stencil > StencilType.None) {
                var value = (int)target.stencil;
                var desc = new StencilDescriptor() {
                    Ref = value.ToString(),
                    Comp = "Always",
                    Pass = "Replace"
                };

                result.Add(RenderState.Stencil(desc));
            }

            if (colorMask) {
                result.Add(RenderState.ColorMask("ColorMask 0"));
            }
            
            return result;
        }
    }
}