using UnityEngine.Rendering;
using UnityEditor.ShaderGraph;

namespace Koiyun.Render.ShaderGraph.Editor {
    static class LaviStructs {
        public static StructDescriptor Varyings = new StructDescriptor() {
            name = "Varyings",
            packFields = true,
            populateWithCustomInterpolators = true,
            fields = new FieldDescriptor[] {
                StructFields.Varyings.positionCS,
                StructFields.Varyings.positionWS,
                StructFields.Varyings.normalWS,
                StructFields.Varyings.tangentWS,
                StructFields.Varyings.texCoord0,
                StructFields.Varyings.texCoord1,
                StructFields.Varyings.texCoord2,
                StructFields.Varyings.texCoord3,
                StructFields.Varyings.color,
                StructFields.Varyings.screenPosition,
                StructFields.Varyings.instanceID,
            }
        };
    }

    enum RenderStateOverride {
        None,
        Current,
        Override
    }

    struct RenderStateParam {
        public Blend srcBlend;
        public Blend dstBlend;
        public CullMode cullMode;
        public ZWrite zWrite;
        public ZTest zTest;
        public StencilType stencilType;
        public bool stencilTest;
        public bool colorMask;
        public RenderStateOverride overrideBlendMode;
        public RenderStateOverride overrideCullMode;
        public RenderStateOverride overrideZWrite;
        public RenderStateOverride overrideZTest;
    }
}