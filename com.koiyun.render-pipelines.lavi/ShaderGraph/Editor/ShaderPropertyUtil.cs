using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

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

        public static KeywordDescriptor ShadowSoftKeyword = new KeywordDescriptor() {
            displayName = "Shadows Soft",
            referenceName = "_SHADOWS_SOFT",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
            stages = KeywordShaderStage.Fragment,
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
            public static BlockFieldDescriptor OutlineColor = new BlockFieldDescriptor(BlockFields.SurfaceDescription.name, "OutlineColor", "Outline Color", "SURFACEDESCRIPTION_OUTLINECOLOR",
                    new ColorControl(UnityEngine.Color.black, false), ShaderStage.Fragment);
        }
    }
}