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
}