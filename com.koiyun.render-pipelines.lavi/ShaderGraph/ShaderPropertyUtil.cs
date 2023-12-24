using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace Koiyun.Render.ShaderGraph.Editor {
    static class ShaderPropertyUtil {
        public static KeywordDescriptor AlphaClipKeyword = new KeywordDescriptor() {
            displayName = "Alpha Clip",
            referenceName = "_ALPHA_CLIP",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Local,
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
            public static BlockFieldDescriptor Color = new BlockFieldDescriptor(BlockFields.SurfaceDescription.name, "Color", "Color", "SURFACEDESCRIPTION_COLOR",
                    new ColorRGBAControl(UnityEngine.Color.clear), ShaderStage.Fragment);

            public static BlockFieldDescriptor Glow = new BlockFieldDescriptor(BlockFields.SurfaceDescription.name, "Glow", "Glow", "SURFACEDESCRIPTION_GLOW",
                    new FloatControl(0), ShaderStage.Fragment);

            public static BlockFieldDescriptor LutUV = new BlockFieldDescriptor(BlockFields.SurfaceDescription.name, "LutUV", "LutUV", "SURFACEDESCRIPTION_LUT_UV",
                    new Vector2Control(UnityEngine.Vector2.zero), ShaderStage.Fragment);
        }

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
    }
}