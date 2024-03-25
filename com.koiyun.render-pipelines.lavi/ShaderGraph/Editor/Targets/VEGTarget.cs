using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor.ShaderGraph;

namespace Koiyun.Render.ShaderGraph.Editor {
    class VEGTarget : Target, IMaySupportVFX {
        public static Dictionary<BlockFieldDescriptor, int> s_BlockMap = new Dictionary<BlockFieldDescriptor, int>() {
            { BlockFields.SurfaceDescription.BaseColor, 1 },
            { BlockFields.SurfaceDescription.Alpha, 2 },
            { ShaderPropertyUtil.SurfaceDescription.Glow, 3 },
        };

        public VEGTarget() {
            displayName = "Visual Effect";
        }

        public override bool IsActive() => true;
        public override void Setup(ref TargetSetupContext context) {}
        public override void GetFields(ref TargetFieldContext context) {}

        public override void GetActiveBlocks(ref TargetActiveBlockContext context) {
            context.AddBlock(BlockFields.SurfaceDescription.BaseColor);
            context.AddBlock(BlockFields.SurfaceDescription.Alpha);
            context.AddBlock(ShaderPropertyUtil.SurfaceDescription.Glow);
        }

        public override void GetPropertiesGUI(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {}

        public override bool WorksWithSRP(RenderPipelineAsset scriptableRenderPipeline) {
            return GraphicsSettings.currentRenderPipeline != null && scriptableRenderPipeline?.GetType() == GraphicsSettings.currentRenderPipeline.GetType();
        }

        public bool SupportsVFX() => true;
        public bool CanSupportVFX() => true;

        public bool HasBlock(BlockFieldDescriptor descriptor, out int slotID) {
            return s_BlockMap.TryGetValue(descriptor, out slotID);
        }
    }
}