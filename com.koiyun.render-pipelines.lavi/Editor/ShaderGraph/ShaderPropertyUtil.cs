using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.Rendering;

namespace Koiyun.Render.ShaderGraph.Editor {
    static class ShaderPropertyUtil {
        public static KeywordDescriptor MainLightShadowsKeyword = new KeywordDescriptor() {
            displayName = "Main Light Shadows",
            referenceName = "_MAIN_LIGHT_SHADOWS",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
            stages = KeywordShaderStage.Fragment,
        };

        public static KeywordDescriptor AlphaClipKeyword = new KeywordDescriptor() {
            displayName = "Alpha Clip",
            referenceName = "_ALPHA_CLIP",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
            stages = KeywordShaderStage.Fragment,
        };

        public static KeywordDescriptor AdditiveKeyword = new KeywordDescriptor() {
            displayName = "Additive",
            referenceName = ShaderGraphConst.ADDITIVE_KEYWORD,
            type = KeywordType.Boolean,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Local,
            stages = KeywordShaderStage.Fragment,
        };

        public static KeywordDescriptor AdditiveKeywordDefined = new KeywordDescriptor() {
            displayName = "Additive",
            referenceName = ShaderGraphConst.ADDITIVE_KEYWORD,
            type = KeywordType.Boolean,
            definition = KeywordDefinition.Predefined
        };

        public static KeywordDescriptor OcclusionKeywordDefined = new KeywordDescriptor() {
            displayName = "Occlusion",
            referenceName = ShaderGraphConst.OCCLUSION_KEYWORD,
            type = KeywordType.Boolean,
            definition = KeywordDefinition.Predefined
        };

        public static KeywordDescriptor NeedAlphaKeyword = new KeywordDescriptor() {
            displayName = "Surface Need Alpha",
            referenceName = "SURFACE_NEED_ALPHA",
            type = KeywordType.Boolean
        };

        public static KeywordDescriptor NeedAlphaClipKeyword = new KeywordDescriptor() {
            displayName = "Surface Need Alpha Clip",
            referenceName = "SURFACE_NEED_ALPHA_CLIP",
            type = KeywordType.Boolean
        };

        public static void AddFloatProperty(PropertyCollector collector, string referenceName, float defaultValue) {
            var property = new Vector1ShaderProperty() {
                floatType = FloatType.Default,
                hidden = true,
                overrideHLSLDeclaration = true,
                hlslDeclarationOverride = HLSLDeclaration.DoNotDeclare,
                value = defaultValue,
                displayName = referenceName,
                overrideReferenceName = referenceName,
            };
            
            collector.AddShaderProperty(property);
        }

        [GenerateBlocks]
        public struct SurfaceDescription {
            public static BlockFieldDescriptor Glow = new BlockFieldDescriptor(BlockFields.SurfaceDescription.name, "Glow", "Glow", "SURFACEDESCRIPTION_GLOW",
                    new FloatControl(0), ShaderStage.Fragment);

            public static BlockFieldDescriptor Layer = new BlockFieldDescriptor(BlockFields.SurfaceDescription.name, "Layer", "Layer", "SURFACEDESCRI   PTION_LAYER",
                    new FloatControl(0), ShaderStage.Fragment);
        }

        public static RenderStateCollection GetRenderState(LaviTarget target, bool noBlend=false, StencilType stencilType=StencilType.None, CompareFunction stencilComp=CompareFunction.Never) {
            target.GetBlend(out var srcBlend, out var dstBlend);

            var overrideBlendMode = target.overrideBlendMode ? RenderStateOverride.Override : RenderStateOverride.Current;

            if (noBlend) {
                overrideBlendMode = RenderStateOverride.None;
            }
            
            var param = new RenderStateParam() {
                srcBlend = srcBlend,
                dstBlend = dstBlend,
                cullMode = target.cullMode,
                zWrite = target.zWrite ? ZWrite.On : ZWrite.Off,
                zTest = target.zTest,
                stencilType = stencilType > StencilType.None ? stencilType : target.stencil,
                stencilComp = stencilComp != CompareFunction.Never ? stencilComp : target.stencilComp,
                colorMask = false,
                overrideBlendMode = overrideBlendMode,
                overrideCullMode = target.overrideCullMode ? RenderStateOverride.Override : RenderStateOverride.Current,
                overrideZWrite = target.overrideZWrite ? RenderStateOverride.Override : RenderStateOverride.Current,
                overrideZTest = target.overrideZTest ? RenderStateOverride.Override : RenderStateOverride.Current,
            };

            return GetRenderState(param);
        }

        public static RenderStateCollection GetRenderState(RenderStateParam param) {
            var result = new RenderStateCollection();

            if (param.overrideBlendMode == RenderStateOverride.Override) {
                var srcBlend = "[" + ShaderGraphConst.SRC_BLEND_PROPERTY + "]";
                var dstBlend = "[" + ShaderGraphConst.DST_BLEND_PROPERTY + "]";
                result.Add(RenderState.Blend(srcBlend, dstBlend));
            }
            else if (param.overrideBlendMode == RenderStateOverride.Current) {
                result.Add(RenderState.Blend(param.srcBlend, param.dstBlend));
            }

            if (param.overrideCullMode == RenderStateOverride.Override) {
                var value = "[" + ShaderGraphConst.CULL_PROPERTY + "]";
                result.Add(RenderState.Cull(value));
            }
            else if (param.overrideCullMode == RenderStateOverride.Current) {
                result.Add(RenderState.Cull(param.cullMode.ToString()));
            }

            if (param.overrideZWrite == RenderStateOverride.Override) {
                var value = "[" + ShaderGraphConst.ZWRITE_PROPERTY + "]";
                result.Add(RenderState.ZWrite(value));
            }
            else if (param.overrideZWrite == RenderStateOverride.Current) {
                result.Add(RenderState.ZWrite(param.zWrite));
            }

            if (param.overrideZTest == RenderStateOverride.Override) {
                var value = "[" + ShaderGraphConst.ZTEST_PROPERTY + "]";
                result.Add(RenderState.ZTest(value));
            }
            else if (param.overrideZTest == RenderStateOverride.Current) {
                result.Add(RenderState.ZTest(param.zTest));
            }

            if (param.stencilType > StencilType.None) {
                var value = (int)param.stencilType;
                var desc = new StencilDescriptor() {
                    Ref = value.ToString(),
                    Comp = param.stencilComp.ToString(),
                    Pass = "Replace"
                };

                result.Add(RenderState.Stencil(desc));
            }

            if (param.colorMask) {
                result.Add(RenderState.ColorMask("ColorMask 0"));
            }

            return result;
        }
        /*
        public static RenderStateCollection GetRenderState(LaviTarget target, bool blendMode, bool cullMode, bool zWrite, bool zTest, bool stencil, bool colorMask) {
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
        */
    }
}