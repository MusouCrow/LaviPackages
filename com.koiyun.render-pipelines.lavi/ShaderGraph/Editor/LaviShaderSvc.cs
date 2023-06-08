using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.ShaderGraph;

namespace Koiyun.Render.ShaderGraph.Editor {
    using Blend = UnityEngine.Rendering.BlendMode;

    static class LaviShaderSvc {
        public static bool IsOpaque(Material material) {
            var renderType = material.GetTag("RenderType", false);

            return renderType != "Transparent";
        }

        public static bool HasBlendMode(Material material) {
            return material.HasProperty(ShaderGraphConst.SRC_BLEND_PROPERTY);
        }

        public static bool HasCullMode(Material material) {
            return material.HasProperty(ShaderGraphConst.CULL_PROPERTY);
        }

        public static bool HasZWrite(Material material) {
            return material.HasProperty(ShaderGraphConst.ZWRITE_PROPERTY);
        }

        public static bool HasZTest(Material material) {
            return material.HasProperty(ShaderGraphConst.ZTEST_PROPERTY);
        }

        public static BlendMode GetBlendMode(Material material) {
            var src = (int)material.GetFloat(ShaderGraphConst.SRC_BLEND_PROPERTY);
            var dst = (int)material.GetFloat(ShaderGraphConst.DST_BLEND_PROPERTY);

            if (src == (int)Blend.One && dst == (int)Blend.One) {
                return BlendMode.Additive;
            }

            return BlendMode.Alpha;
        }

        public static CullMode GetCullMode(Material material) {
            var value = (int)material.GetFloat(ShaderGraphConst.CULL_PROPERTY);

            return (CullMode)value;
        }

        public static bool GetZWrite(Material material) {
            var value = material.GetFloat(ShaderGraphConst.ZWRITE_PROPERTY);

            return value > 0;
        }

        public static ZTest GetZTest(Material material) {
            var value = (int)material.GetFloat(ShaderGraphConst.ZTEST_PROPERTY);

            return (ZTest)value;
        }

        public static void SetBlendMode(Material material, BlendMode blendMode) {
            if (!HasBlendMode(material)) {
                return;
            }

            var src = Blend.One;
            var dst = Blend.Zero;
            var isOpaque = IsOpaque(material);

            if (!isOpaque) {
                if (blendMode == BlendMode.Additive) {
                    src = Blend.One;
                    dst = Blend.One;
                }
                else {
                    src = Blend.SrcAlpha;
                    dst = Blend.OneMinusSrcAlpha;
                }
            }

            material.SetFloat(ShaderGraphConst.SRC_BLEND_PROPERTY, (float)src);
            material.SetFloat(ShaderGraphConst.DST_BLEND_PROPERTY, (float)dst);
        }

        public static void SetCullMode(Material material, CullMode cullMode) {
            if (!HasCullMode(material)) {
                return;
            }

            material.SetFloat(ShaderGraphConst.CULL_PROPERTY, (int)cullMode);
        }

        public static void SetZWrite(Material material, bool zWrite) {
            if (!HasZWrite(material)) {
                return;
            }

            material.SetFloat(ShaderGraphConst.ZWRITE_PROPERTY, zWrite ? 1 : 0);
        }

        public static void SetZTest(Material material, ZTest zTest) {
            if (!HasZTest(material)) {
                return;
            }

            material.SetFloat(ShaderGraphConst.ZTEST_PROPERTY, (int)zTest);
        }
    }
}